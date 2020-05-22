using System;
using System.Runtime.InteropServices;

namespace Mavanmanen.PPMC.MIDI
{
    /// <summary>
    /// Represents a virtual MIDI device.
    /// </summary>
	public class VirtualMidiDevice
	{
        private IntPtr _instance;

        /// <summary>
        /// Create a new virtual MIDI device with the specified name.
        /// </summary>
        /// <param name="portName">Name for the MIDI device.</param>
		public VirtualMidiDevice(string portName)
		{
			_instance = VirtualMIDICreatePortEx2(portName, IntPtr.Zero, IntPtr.Zero, 65535, 1);

            if (_instance == IntPtr.Zero)
            {
                throw new VirtualMIDIException(Marshal.GetLastWin32Error());
            }
        }

        ~VirtualMidiDevice()
        {
            if (_instance == IntPtr.Zero)
            {
                return;
            }

            VirtualMIDIClosePort(_instance);
            _instance = IntPtr.Zero;
        }

        /// <summary>
        /// Send data to the virtual MIDI device.
        /// </summary>
        /// <param name="midiChannel">The MIDI channel to send the data on.</param>
        /// <param name="data1">Value for the first data segment.</param>
        /// <param name="data2">Value for the second data segment.</param>
        public void SendData(int midiChannel, int data1, int data2)
        { 
            var msg = 0;
            msg = (msg & ~240) | 0xB0;
            msg = (msg & ~15) | midiChannel;
            msg = (msg & ~65280) | (data1 << 8);
            msg = (msg & (~65280 + ~255)) | (data2 << (16));

            var bytes = BitConverter.GetBytes(msg);
            if (!VirtualMIDISendData(_instance, bytes, (uint)bytes.Length))
            {
                throw new VirtualMIDIException(Marshal.GetLastWin32Error());
            }
        }

        /// <summary>
        /// Close the virtual MIDI device connection.
        /// </summary>
        public void Close() => VirtualMIDIClosePort(_instance);

        private const string DllName = "teVirtualMIDI.dll";

        [DllImport(DllName, EntryPoint = "virtualMIDICreatePortEx2", SetLastError = true, CharSet = CharSet.Unicode)]
		private static extern IntPtr VirtualMIDICreatePortEx2(string portName, IntPtr callback, IntPtr dwCallbackInstance, uint maxSysexLength, uint flags);

		[DllImport(DllName, EntryPoint = "virtualMIDIClosePort", SetLastError = true, CharSet = CharSet.Unicode)]
		private static extern void VirtualMIDIClosePort(IntPtr instance);

		[DllImport(DllName, EntryPoint = "virtualMIDISendData", SetLastError = true, CharSet = CharSet.Unicode)]
		private static extern bool VirtualMIDISendData(IntPtr midiPort, byte[] midiDataBytes, uint length);
	}
}
