using System;
using System.Runtime.InteropServices;
using DirectShowLib;

namespace Minimal_video_player_DirectShow_C_Sharp
{
    public static class DirectShowUtils
    {
        public const int S_OK = 0;
        public const int S_FALSE = 1;
        public const int E_POINTER = -2147467261; //0x80004003

        public static readonly Guid CLSID_FileSourceAsync = new Guid("{E436EBB5-524F-11CE-9F53-0020AF0BA770}");
        public static readonly Guid CLSID_LAV_Splitter = new Guid("{171252A0-8820-4AFE-9DF8-5C92B2D66B04}");
        public static readonly Guid CLSID_LAV_VideoDecoder = new Guid("{EE30215D-164F-4A92-A4EB-9D4C13390F9F}");
        public static readonly Guid CLSID_VideoRenderer = new Guid("{70E102B0-5556-11CE-97C0-00AA0055595A}");
        public static readonly Guid CLSID_LAV_AudioDecoder = new Guid("{E8E73B6B-4CB3-44A4-BE99-4F7BCB96E491}");
        public static readonly Guid CLSID_DirectSoundAudioRenderer = new Guid("{79376820-07D0-11CF-A24D-0020AFD79767}");

        public static int FindPin(IBaseFilter filter, int pinId, PinDirection pinDirection, out IPin resultPin)
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

        public static int FindPin(IBaseFilter filter, string pinName, PinDirection pinDirection, out IPin resultPin)
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

        public static int CreateComObject<T, T2>(out T2 obj) where T : new()
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

        public static int CreateDirectShowFilter(Guid guid, out IBaseFilter filter)
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
