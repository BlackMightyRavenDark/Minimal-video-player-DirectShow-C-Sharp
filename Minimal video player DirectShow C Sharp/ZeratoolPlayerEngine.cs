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
        private IMediaControl mediaControl = null;
        private IMediaPosition mediaPosition = null;

        private IFileSourceFilter fileSource = null;
        private IBaseFilter fileSourceFilter = null;
        private IBaseFilter mediaSplitter = null;
        private IBaseFilter videoDecoder = null;
        private IBaseFilter videoRenderer = null;

        public const int ERROR_FILE_NAME_NOT_DEFINED = -100;
        public const int ERROR_FILE_NOT_FOUND = -101;
        public const int ERROR_VIDEO_OUTPUT_WINDOW_NOT_DEFINED = -102;

        public string FileName { get; set; }
        public Control VideoOutputWindow { get; set; }
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
            FindPin(fileSourceFilter, 0, PinDirection.Output, out IPin pinOut);
            FindPin(mediaSplitter, 0, PinDirection.Input, out IPin pinIn);
            errorCode = graphBuilder.Connect(pinOut, pinIn);
            if (errorCode != S_OK)
            {
                Clear();
                return errorCode;
            }

            CreateDirectShowFilter(CLSID_LAV_VideoDecoder, out videoDecoder);
            graphBuilder.AddFilter(videoDecoder, "Video decoder");
            FindPin(mediaSplitter, "ideo", PinDirection.Output, out pinOut);
            FindPin(videoDecoder, 0, PinDirection.Input, out pinIn);
            errorCode = graphBuilder.Connect(pinOut, pinIn);
            if (errorCode != S_OK)
            {
                Clear();
                return errorCode;
            }

            CreateDirectShowFilter(CLSID_VideoRenderer, out videoRenderer);
            graphBuilder.AddFilter(videoRenderer, "Video renderer");
            FindPin(videoDecoder, 0, PinDirection.Output, out pinOut);
            FindPin(videoRenderer, 0, PinDirection.Input, out pinIn);
            errorCode = graphBuilder.Connect(pinOut, pinIn);
            if (errorCode != S_OK)
            {
                Clear();
                return errorCode;
            }

            videoWindow = (IVideoWindow)graphBuilder;
            videoWindow.put_Owner(VideoOutputWindow.Handle);
            videoWindow.put_MessageDrain(VideoOutputWindow.Handle);
            videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren | WindowStyle.ClipSiblings);
            SetVideoOutputRectangle(VideoOutputWindow.ClientRectangle);
            videoWindow.put_Visible(OABool.True);

            mediaPosition = (IMediaPosition)graphBuilder;

            mediaControl = (IMediaControl)graphBuilder;
            mediaControl.Run();

            return S_OK;

        }

        public void SetVideoOutputRectangle(Rectangle rectangle)
        {
            videoWindow.SetWindowPosition(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
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
    }
}
