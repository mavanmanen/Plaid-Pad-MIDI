using System;

namespace Mavanmanen.PPM.HID
{
    public class HIDDataReceivedEventArgs : EventArgs
    {
        public byte[] Data { get; }

        public HIDDataReceivedEventArgs(byte[] data)
        {
            Data = data;
        }
    }
}
