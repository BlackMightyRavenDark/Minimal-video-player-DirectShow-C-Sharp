using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DirectShowLib;
using static Minimal_video_player_DirectShow_C_Sharp.DirectShowUtils;

namespace Minimal_video_player_DirectShow_C_Sharp
{
    public class ZeratoolPlayerEngine
    {
        private IGraphBuilder graphBuilder = null;
        private IVideoWindow videoWindow = null;
        private IBasicVideo basicVideo = null;
        private IBasicAudio basicAudio = null;
        private IMediaControl mediaControl = null;
        private IMediaPosition mediaPosition = null;

        private IFileSourceFilter fileSource = null;
        private IBaseFilter fileSourceFilter = null;
        private IBaseFilter mediaSplitter = null;
        private IBaseFilter videoDecoder = null;
        private IBaseFilter videoRenderer = null;
        private IBaseFilter audioDecoder = null;
        private IBaseFilter audioRenderer = null;

        public const int ERROR_FILE_NAME_NOT_DEFINED = -100;
        public const int ERROR_FILE_NOT_FOUND = -101;
        public const int ERROR_VIDEO_OUTPUT_WINDOW_NOT_DEFINED = -102;
        public const int ERROR_NOTHING_RENDERED = -103;

        private int _videoWidth;
        private int _videoHeight;
        private int _volume = 25;

        public string FileName { get; set; }
        public Control VideoOutputWindow { get; set; }
        public Size VideoSize { get { return new Size(_videoWidth, _videoHeight); } }
        public double Position
        {
            get
            {
                double pos = 0.0;
                if (mediaPosition != null)
                {
                    mediaPosition.get_CurrentPosition(out pos);
                }
                return pos;
            }
            set
            {
                if (mediaPosition != null)
                {
                    mediaPosition.put_CurrentPosition(value);
                }
            }
        }

        public double Duration
        {
            get
            {
                double dur = 0.0;
                if (mediaPosition != null)
                {
                    mediaPosition.get_Duration(out dur);
                }
                return dur;
            }
        }

        public int Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                else if (value > 100)
                {
                    value = 100;
                }
                if (_volume != value)
                {
                    _volume = value;
                    if (basicAudio != null)
                    {
                        int db = GetDecibelsVolume(_volume);
                        basicAudio.put_Volume(db);
                    }
                }
            }
        }

        public bool AudioRendered => basicAudio != null;
        public bool VideoRendered => basicVideo != null;
        

        public int BuildGraph()
        {
            if (string.IsNullOrEmpty(FileName) || string.IsNullOrWhiteSpace(FileName))
            {
                return ERROR_FILE_NAME_NOT_DEFINED;
            }
            if (!File.Exists(FileName))
            {
                return ERROR_FILE_NOT_FOUND;
            }
            if (VideoOutputWindow == null)
            {
                return ERROR_VIDEO_OUTPUT_WINDOW_NOT_DEFINED;
            }

            CreateComObject<FilterGraph, IGraphBuilder>(out graphBuilder);
            CreateDirectShowFilter(CLSID_FileSourceAsync, out fileSourceFilter);
            graphBuilder.AddFilter(fileSourceFilter, "Source filter");
            fileSource = (IFileSourceFilter)fileSourceFilter;

            int errorCode = fileSource.Load(FileName, null);
            if (errorCode != S_OK)
            {
                Clear();
                return errorCode;
            }

            CreateDirectShowFilter(CLSID_LAV_Splitter, out mediaSplitter);
            graphBuilder.AddFilter(mediaSplitter, "Media splitter");
            if (FindPin(fileSourceFilter, 0, PinDirection.Output, out IPin pinOut) != S_OK)
            {
                Clear();
                return E_POINTER;
            }
            if (FindPin(mediaSplitter, 0, PinDirection.Input, out IPin pinIn) != S_OK)
            {
                Marshal.ReleaseComObject(pinOut);
                Clear();
                return E_POINTER;
            }
            errorCode = graphBuilder.Connect(pinOut, pinIn);
            Marshal.ReleaseComObject(pinIn);
            Marshal.ReleaseComObject(pinOut);
            if (errorCode != S_OK)
            {
                Clear();
                return errorCode;
            }

            int errorCodeVideo = S_FALSE;
            if (FindPin(mediaSplitter, "ideo", PinDirection.Output, out pinOut) == S_OK)
            {
                errorCodeVideo = RenderVideoStream_Manual(pinOut);
                Marshal.ReleaseComObject(pinOut);
                if (errorCodeVideo != S_OK)
                {
                    ClearVideoChain();
                }
            }

            int errorCodeAudio = S_FALSE;
            if (FindPin(mediaSplitter, "udio", PinDirection.Output, out pinOut) == S_OK)
            {
                errorCodeAudio = RenderAudioStream_Manual(pinOut);
                Marshal.ReleaseComObject(pinOut);
                if (errorCodeAudio != S_OK)
                {
                    ClearAudioChain();
                }
            }

            if (errorCodeVideo != S_OK && errorCodeAudio != S_OK)
            {
                Clear();
                return ERROR_NOTHING_RENDERED;
            }

            videoWindow = (IVideoWindow)graphBuilder;
            videoWindow.put_Owner(VideoOutputWindow.Handle);
            videoWindow.put_MessageDrain(VideoOutputWindow.Handle);
            videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren | WindowStyle.ClipSiblings);

            if (GetComInterface<IBasicVideo>(graphBuilder, out basicVideo) &&
                basicVideo.GetVideoSize(out _videoWidth, out _videoHeight) == S_OK)
            {
                Rectangle videoRect = new Rectangle(0, 0, _videoWidth, _videoHeight);
                videoRect = videoRect.ResizeTo(VideoOutputWindow.ClientSize).CenterIn(VideoOutputWindow.ClientRectangle);
                SetVideoOutputRectangle(videoRect);
                videoWindow.put_Visible(OABool.True);
            }
            else
            {
                ClearVideoChain();
            }

            if (GetComInterface<IBasicAudio>(graphBuilder, out basicAudio))
            {
                int db = GetDecibelsVolume(Volume);
                basicAudio.put_Volume(db);
            }
            else
            {
                ClearAudioChain();
            }

            if (!VideoRendered && !AudioRendered)
            {
                Clear();
                return ERROR_NOTHING_RENDERED;
            }

            mediaPosition = (IMediaPosition)graphBuilder;

            mediaControl = (IMediaControl)graphBuilder;
            mediaControl.Run();

            return S_OK;

        }

        private int RenderVideoStream_Manual(IPin splitterPinOut)
        {
            CreateDirectShowFilter(CLSID_LAV_VideoDecoder, out videoDecoder);
            graphBuilder.AddFilter(videoDecoder, "Video decoder");
            if (FindPin(videoDecoder, 0, PinDirection.Input, out IPin pinIn) != S_OK)
            {
                graphBuilder.RemoveFilter(videoDecoder);
                return E_POINTER;
            }
            int errorCode = graphBuilder.Connect(splitterPinOut, pinIn);
            Marshal.ReleaseComObject(pinIn);
            if (errorCode != S_OK)
            {
                graphBuilder.RemoveFilter(videoDecoder);
                return errorCode;
            }

            CreateDirectShowFilter(CLSID_VideoMixingRenderer9, out videoRenderer);
            graphBuilder.AddFilter(videoRenderer, "Video renderer");
            if (FindPin(videoDecoder, 0, PinDirection.Output, out IPin pinOut) != S_OK)
            {
                graphBuilder.RemoveFilter(videoRenderer);
                graphBuilder.RemoveFilter(videoDecoder);
                return E_POINTER;
            }
            if (FindPin(videoRenderer, 0, PinDirection.Input, out pinIn) != S_OK)
            {
                graphBuilder.RemoveFilter(videoRenderer);
                graphBuilder.RemoveFilter(videoDecoder);
                Marshal.ReleaseComObject(pinOut);
                return E_POINTER;
            }
            errorCode = graphBuilder.Connect(pinOut, pinIn);
            Marshal.ReleaseComObject(pinIn);
            Marshal.ReleaseComObject(pinOut);
            if (errorCode != S_OK)
            {
                graphBuilder.RemoveFilter(audioRenderer);
                graphBuilder.RemoveFilter(videoDecoder);
            }

            return errorCode;
        }

        private int RenderAudioStream_Manual(IPin splitterPinOut)
        {
            int errorCode = CreateDirectShowFilter(CLSID_DirectSoundAudioRenderer, out audioRenderer);
            if (errorCode != S_OK)
            {
                return errorCode;
            }
            graphBuilder.AddFilter(audioRenderer, "Audio renderer");

            errorCode = CreateDirectShowFilter(CLSID_LAV_AudioDecoder, out audioDecoder);
            if (errorCode != S_OK)
            {
                graphBuilder.RemoveFilter(audioRenderer);
                return errorCode;
            }
            graphBuilder.AddFilter(audioDecoder, "Audio decoder");

            if (FindPin(audioDecoder, 0, PinDirection.Input, out IPin pinIn) != S_OK)
            {
                graphBuilder.RemoveFilter(audioRenderer);
                graphBuilder.RemoveFilter(audioDecoder);
                return E_POINTER;
            }
            errorCode = graphBuilder.Connect(splitterPinOut, pinIn);
            Marshal.ReleaseComObject(pinIn);
            if (errorCode != S_OK)
            {
                graphBuilder.RemoveFilter(audioRenderer);
                graphBuilder.RemoveFilter(audioDecoder);
                return errorCode;
            }

            if (FindPin(audioDecoder, 0, PinDirection.Output, out IPin pinOut) != S_OK)
            {
                graphBuilder.RemoveFilter(audioRenderer);
                graphBuilder.RemoveFilter(audioDecoder);
                return E_POINTER;
            }
            if (FindPin(audioRenderer, 0, PinDirection.Input, out pinIn) != S_OK)
            {
                graphBuilder.RemoveFilter(audioRenderer);
                graphBuilder.RemoveFilter(audioDecoder);
                Marshal.ReleaseComObject(pinOut);
                return E_POINTER;
            }
            errorCode = graphBuilder.Connect(pinOut, pinIn);
            Marshal.ReleaseComObject(pinIn);
            Marshal.ReleaseComObject(pinOut);
            if (errorCode != S_OK)
            {
                graphBuilder.RemoveFilter(audioRenderer);
                graphBuilder.RemoveFilter(audioDecoder);
            }
            return errorCode;
        }

        public void SetVideoOutputRectangle(Rectangle rectangle)
        {
            videoWindow.SetWindowPosition(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        private void ClearVideoChain()
        {
            if (videoWindow != null)
            {
                Marshal.ReleaseComObject(videoWindow);
                videoWindow = null;
            }

            if (videoRenderer != null)
            {
                Marshal.ReleaseComObject(videoRenderer);
                videoRenderer = null;
            }

            if (videoDecoder != null)
            {
                Marshal.ReleaseComObject(videoDecoder);
                videoDecoder = null;
            }

            if (basicVideo != null)
            {
                Marshal.ReleaseComObject(basicVideo);
                basicVideo = null;
            }

            _videoWidth = 0;
            _videoHeight = 0;
        }

        private void ClearAudioChain()
        {
            if (basicAudio != null)
            {
                Marshal.ReleaseComObject(basicAudio);
                basicAudio = null;
            }

            if (audioDecoder != null)
            {
                Marshal.ReleaseComObject(audioDecoder);
                audioDecoder = null;
            }

            if (audioRenderer != null)
            {
                Marshal.ReleaseComObject(audioRenderer);
                audioRenderer = null;
            }
        }

        public void Clear()
        {
            if (mediaControl != null)
            {
                mediaControl.Stop();
                Marshal.ReleaseComObject(mediaControl);
                mediaControl = null;
            }

            if (mediaPosition != null)
            {
                Marshal.ReleaseComObject(mediaPosition);
                mediaPosition = null;
            }


            ClearVideoChain();
            ClearAudioChain();

            if (mediaSplitter != null)
            {
                Marshal.ReleaseComObject(mediaSplitter);
                mediaSplitter = null;
            }

            if (fileSource != null)
            {
                Marshal.ReleaseComObject(fileSource);
                fileSource = null;
            }

            if (fileSourceFilter != null)
            {
                Marshal.ReleaseComObject(fileSourceFilter);
                fileSourceFilter = null;
            }

            if (graphBuilder != null)
            {
                Marshal.ReleaseComObject(graphBuilder);
                graphBuilder = null;
            }
        }

        public static int GetDecibelsVolume(int volume)
        {
            long vol = volume * ushort.MaxValue / 100;
            int db = (int)Math.Truncate(100 * 33.22 * Math.Log((vol + 1e-6) / ushort.MaxValue) / Math.Log(10));
            if (db < -10000)
            {
                db = -10000;
            }
            else if (db > 0)
            {
                db = 0;
            }
            return db;
        }
    }
}
