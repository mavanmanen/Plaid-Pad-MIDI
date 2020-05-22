using System;
using System.Runtime.CompilerServices;
using Mavanmanen.PPM.HID;
using Mavanmanen.PPMC.MIDI;

namespace Mavanmanen.PPM
{
    public static class Engine
    {
        private enum InputType
        {
            Undefined,
            Encoder,
            Button
        }

        private static VirtualMidiDevice _midiDevice;
        public static PadState CurrentState { get; set; } = new PadState();

        public static event EventHandler OnConnected;
        public static event EventHandler OnDisconnected;
        public static void Start()
        {
            HIDManager.OnConnected += HIDManager_OnConnected;
            HIDManager.OnDisconnected += HIDManager_OnDisconnected;
            HIDManager.OnHidDataReceived += HIDManager_OnHidDataReceived;
            HIDManager.WaitForDevice(0xFEED, 0xAF12, 0xFF60);
            _midiDevice = new VirtualMidiDevice("Plaid Pad MIDI");
        }

        public static void Stop()
        {
            SendRawHIDMessage(RawHIDMessage.HID_DISCONNECTED);
            HIDManager.StopListening();
        }

        public static PadState GetState()
        {
            var stateData = SendRawHIDMessage(RawHIDMessage.HID_GET_STATE, true);
            CurrentState.LoadFromByteData(stateData);
            return CurrentState;
        }

        private static void HIDManager_OnHidDataReceived(object sender, HIDDataReceivedEventArgs e)
        {
            if (e.Data[1] >= (byte)RawHIDMessage.HID_ACK)
            {
                return;
            }

            var type = e.Data[1];
            var index = e.Data[2];
            var value = e.Data[3];

            switch ((InputType)type)
            {
                case InputType.Encoder:
                    if (index == 0)
                    {
                        CurrentState.EncoderLeft = value;
                    }
                    else
                    {
                        CurrentState.EncoderRight = value;
                    }
                    break;

                case InputType.Button:
                    CurrentState.SetButton(index, value != 0);
                    break;

                default:
                    return;
            }

            _midiDevice.SendData(type - 1, index, value);
        }

        private static void HIDManager_OnDisconnected(object sender, EventArgs e)
        {
            HIDManager.StopListening();
            OnDisconnected?.Invoke(typeof(Engine), new EventArgs());
        }

        private static void HIDManager_OnConnected(object sender, EventArgs e)
        {
            CurrentState = GetState();
            HIDManager.StartListening();
            SendRawHIDMessage(RawHIDMessage.HID_CONNECTED);
            OnConnected?.Invoke(typeof(Engine), new EventArgs());
        }

        private static byte[] SendRawHIDMessage(RawHIDMessage message, bool waitForReply = false, byte[] messageData = null)
        {
            var data = new byte[32];
            data[1] = (byte)message;

            if (messageData != null)
            {
                for (var i = 0; i < messageData.Length; i++)
                {
                    data[i + 1] = messageData[i];
                }
            }

            if (waitForReply)
            {
                return HIDManager.SendDataWaitForReply(data);
            }

            HIDManager.SendData(data);
            return null;
        }
    }
}