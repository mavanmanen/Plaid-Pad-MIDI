using System;

namespace Mavanmanen.PPM.HID
{
    /// <summary>
    /// Holds data for when data is received from a HID device.
    /// </summary>
    public class HIDDataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// The raw data that was received from the HID device.
        /// </summary>
        public byte[] Data { get; }

        public HIDDataReceivedEventArgs(byte[] data)
        {
            Data = data;
        }
    }
}
