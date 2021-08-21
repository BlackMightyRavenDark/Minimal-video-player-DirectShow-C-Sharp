using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using DirectShowLib;

namespace Minimal_video_player_DirectShow_C_Sharp
{
    public partial class Form1 : Form
    {
        private const int S_OK = 0;
        private const int S_FALSE = 1;

        private readonly Guid CLSID_FileSourceAsync = new Guid("{E436EBB5-524F-11CE-9F53-0020AF0BA770}");
        private readonly Guid CLSID_LAV_Splitter = new Guid("{171252A0-8820-4AFE-9DF8-5C92B2D66B04}");
        private readonly Guid CLSID_LAV_VideoDecoder = new Guid("{EE30215D-164F-4A92-A4EB-9D4C13390F9F}");
        private readonly Guid CLSID_VideoRenderer = new Guid("{70E102B0-5556-11CE-97C0-00AA0055595A}");

        private IGraphBuilder graphBuilder = null;
        private IVideoWindow videoWindow = null;
        private IMediaControl mediaControl = null;

        private IFileSourceFilter fileSource = null;
        private IBaseFilter fileSourceFilter = null;
        private IBaseFilter mediaSplitter = null;
        private IBaseFilter videoDecoder = null;
        private IBaseFilter videoRenderer = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string fn = "video.avi";
            if (string.IsNullOrEmpty(fn) || !File.Exists(fn))
            {
                MessageBox.Show($"File not found!{Environment.NewLine}{fn}");
                return;
            }
            BuildGraphManual(fn);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ClearGraph();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (videoWindow != null)
            {
                videoWindow.SetWindowPosition(0, 0, panelVideoOutput.Width, panelVideoOutput.Height);
            }
        }

        private int BuildGraphManual(string fileName)
        {
            CreateComObject<FilterGraph, IGraphBuilder>(out graphBuilder);
            CreateDirectShowFilter(CLSID_FileSourceAsync, out fileSourceFilter);
            graphBuilder.AddFilter(fileSourceFilter, "Source filter");
            fileSource = (IFileSourceFilter)fileSourceFilter;
            
            int errorCode = fileSource.Load(fileName, null);
            if (errorCode != S_OK)
            {
                ClearGraph();
                return errorCode;
            }

            CreateDirectShowFilter(CLSID_LAV_Splitter, out mediaSplitter);
            graphBuilder.AddFilter(mediaSplitter, "Media splitter");
            FindPin(fileSourceFilter, 0, PinDirection.Output, out IPin pinOut);
            FindPin(mediaSplitter, 0, PinDirection.Input, out IPin pinIn);
            errorCode = graphBuilder.Connect(pinOut, pinIn);
            if (errorCode != S_OK)
            {
                ClearGraph();
                return errorCode;
            }

            CreateDirectShowFilter(CLSID_LAV_VideoDecoder, out videoDecoder);
            graphBuilder.AddFilter(videoDecoder, "Video decoder");
            FindPin(mediaSplitter, "ideo", PinDirection.Output, out pinOut);
            FindPin(videoDecoder, 0, PinDirection.Input, out pinIn);
            errorCode = graphBuilder.Connect(pinOut, pinIn);
            if (errorCode != S_OK)
            {
                ClearGraph();
                return errorCode;
            }

            CreateDirectShowFilter(CLSID_VideoRenderer, out videoRenderer);
            graphBuilder.AddFilter(videoRenderer, "Video renderer");
            FindPin(videoDecoder, 0, PinDirection.Output, out pinOut);
            FindPin(videoRenderer, 0, PinDirection.Input, out pinIn);
            errorCode = graphBuilder.Connect(pinOut, pinIn);
            if (errorCode != S_OK)
            {
                ClearGraph();
                return errorCode;
            }

            videoWindow = (IVideoWindow)graphBuilder;
            videoWindow.put_Owner(panelVideoOutput.Handle);
            videoWindow.put_MessageDrain(panelVideoOutput.Handle);
            videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren | WindowStyle.ClipSiblings);
            videoWindow.SetWindowPosition(0, 0, panelVideoOutput.Width, panelVideoOutput.Height);
            videoWindow.put_Visible(OABool.True);

            mediaControl = (IMediaControl)graphBuilder;
            mediaControl.Run();

            return S_OK;
        }

        private void ClearGraph()
        {
            if (mediaControl != null)
            {
                mediaControl.Stop();
                Marshal.ReleaseComObject(mediaControl);
                mediaControl = null;
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

        private int FindPin(IBaseFilter filter, int pinId, PinDirection pinDirection, out IPin resultPin)
        {
            if (filter != null && filter.EnumPins(out IEnumPins enumPins) == S_OK)
            {
                int id = 0;
                IPin[] pins = new IPin[1];
                while (enumPins.Next(1, pins, new IntPtr(0)) == S_OK)
                {
                    if (pins[0].QueryDirection(out PinDirection dir) == S_OK && dir == pinDirection)
                    {
                        if (pinId == id)
                        {
                            Marshal.ReleaseComObject(enumPins);
                            resultPin = pins[0];
                            return S_OK;
                        }
                        id++;
                    }
                    Marshal.ReleaseComObject(pins[0]);
                }
                Marshal.ReleaseComObject(enumPins);
            }

            resultPin = null;
            return S_FALSE;
        }

        private int FindPin(IBaseFilter filter, string pinName, PinDirection pinDirection, out IPin resultPin)
        {
            if (filter != null && filter.EnumPins(out IEnumPins enumPins) == S_OK)
            {
                IPin[] pins = new IPin[1];
                while (enumPins.Next(1, pins, new IntPtr(0)) == S_OK)
                {
                    if (pins[0].QueryPinInfo(out PinInfo pinInfo) == S_OK &&
                        pinInfo.dir == pinDirection && pinInfo.name.Contains(pinName))
                    {
                        Marshal.ReleaseComObject(enumPins);
                        resultPin = pins[0];
                        return S_OK;
                    }
                    Marshal.ReleaseComObject(pins[0]);
                }
                Marshal.ReleaseComObject(enumPins);
            }

            resultPin = null;
            return S_FALSE;
        }

        private int CreateComObject<T, T2>(out T2 obj) where T : new()
        {
            try
            {
                object o = new T();
                obj = (T2)o;
                return S_OK;
            }
            catch (Exception ex)
            {
                obj = default;
                return ex.HResult;
            }
        }

        private int CreateDirectShowFilter(Guid guid, out IBaseFilter filter)
        {
            Type type = Type.GetTypeFromCLSID(guid);
            try
            {
                filter = (IBaseFilter)Activator.CreateInstance(type);
                return S_OK;
            }
            catch (Exception ex)
            {
                filter = null;
                return ex.HResult;
            }
        }

    }
}
