using System;
using System.Runtime.InteropServices;

namespace Mavanmanen.PPMC.MIDI
{
	public class VirtualMidiDevice
	{
        private IntPtr _instance;

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
