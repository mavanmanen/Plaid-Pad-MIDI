using System;
using System.Linq;
using System.Threading;

using HidLibrary;

namespace Mavanmanen.PPM.HID
{
    public static class HIDManager
    {
        private static HidDevice _connectedDevice;
        private static Thread _listeningThread;
        private static bool _abortThread;

        public static event EventHandler OnConnected;
        public static event EventHandler OnDisconnected;
        public static event EventHandler<HIDDataReceivedEventArgs> OnHidDataReceived;

        public static void WaitForDevice(int vendorId, int productId, ushort usagePage)
        {
            while(_connectedDevice == null)
            {
                _connectedDevice = HidDevices.Enumerate(vendorId, productId, usagePage)
                    .FirstOrDefault();

                Thread.Sleep(10);
            }

            _connectedDevice.Removed += () => OnDisconnected?.Invoke(_connectedDevice, new EventArgs());
            _connectedDevice.Inserted += () => OnConnected?.Invoke(_connectedDevice, new EventArgs());
            _connectedDevice.MonitorDeviceEvents = true;
        }

        public static void StartListening()
        {
            _connectedDevice.OpenDevice();
            StartListeningThread();
        }

        public static void StopListening()
        {
            StopListeningThread();
            _connectedDevice.CloseDevice();
        }

        public static bool SendData(byte[] data)
        {
            return _connectedDevice.Write(data);
        }

        public static byte[] SendDataWaitForReply(byte[] data)
        {
            StopListeningThread();
            var retVal = new byte[32];

            if(SendData(data))
            {
                retVal = _connectedDevice.Read().Data;
            }

            StartListeningThread();
            return retVal;
        }

        private static void StartListeningThread()
        {
            if (_listeningThread?.IsAlive == true)
            {
                return;
            }

            _listeningThread = new Thread(ListeningLoop)
            {
                IsBackground = true
            };

            _listeningThread.Start();
        }

        private static void StopListeningThread()
        {
            if (_listeningThread?.IsAlive != true)
            {
                return;
            }

            _abortThread = true;
            _listeningThread.Join(5000);
            if(_listeningThread.IsAlive)
            {
                _listeningThread.Abort();
            }
        }

        private static void ListeningLoop()
        {
            while (!_abortThread)
            {
                var data = _connectedDevice.Read();
                if(data.Data.Length > 0)
                {
                    OnHidDataReceived?.Invoke(_connectedDevice, new HIDDataReceivedEventArgs(data.Data));
                }
            }

            _abortThread = false;
        }
    }
}
