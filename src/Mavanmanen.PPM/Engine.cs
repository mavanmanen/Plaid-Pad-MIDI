using System;
using Mavanmanen.PPM.HID;

using Mavanmanen.PPMC.MIDI;

namespace Mavanmanen.PPM
{
    /// <summary>
    /// Handles all HID and MIDI communication.
    /// </summary>
    public static class Engine
    {
        /// <summary>
        /// The different input types supported by the Plaid Pad.
        /// </summary>
        private enum InputType
        {
            /// <summary>
            /// Undefined input type, something is wrong on the firmware end.
            /// </summary>
            Undefined,

            /// <summary>
            /// Rotary encoder.
            /// </summary>
            Encoder,

            /// <summary>
            /// Keyboard switch.
            /// </summary>
            Button
        }

        private static VirtualMidiDevice _midiDevice;

        /// <summary>
        /// Current state of the Plaid Pad.
        /// </summary>
        public static PadState CurrentState { get; set; } = new PadState();

        /// <summary>
        /// Invoked when the Plaid Pad is connected.
        /// </summary>
        public static event EventHandler OnConnected;

        /// <summary>
        /// Invoked when the Plaid Pad is disconnected.
        /// </summary>
        public static event EventHandler OnDisconnected;

        /// <summary>
        /// Starts this engine. This will wait till the correct HID device is connected and create the virtual MIDI device.
        /// </summary>
        public static void Start()
        {
            HIDManager.OnConnected += HIDManager_OnConnected;
            HIDManager.OnDisconnected += HIDManager_OnDisconnected;
            HIDManager.OnHidDataReceived += HIDManager_OnHidDataReceived;
            HIDManager.WaitForDevice(0xFEED, 0xAF12, 0xFF60);
            _midiDevice = new VirtualMidiDevice("Plaid Pad MIDI");
        }

        /// <summary>
        /// Stops the engine. This will disconnect with the HID device.
        /// </summary>
        public static void Stop()
        {
            SendRawHIDMessage(RawHIDMessage.HID_DISCONNECTED);
            HIDManager.StopListening();
        }

        /// <summary>
        /// Request the current state from the Plaid Pad.
        /// </summary>
        /// <returns>The current state.</returns>
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