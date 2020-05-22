using System;
using System.Linq;
using System.Threading;

using HidLibrary;

namespace Mavanmanen.PPM.HID
{
    /// <summary>
    /// Manages a connection to a HID device and will fire off events when data is received or when the device is either connected or disconnected.
    /// </summary>
    public static class HIDManager
    {
        private static HidDevice _connectedDevice;
        private static Thread _listeningThread;
        private static bool _abortThread;

        /// <summary>
        /// Fired when the device has connected.
        /// </summary>
        public static event EventHandler OnConnected;

        /// <summary>
        /// Fired when the device has disconnected.
        /// </summary>
        public static event EventHandler OnDisconnected;

        /// <summary>
        /// Fire when data has been received from the device.
        /// </summary>
        public static event EventHandler<HIDDataReceivedEventArgs> OnHidDataReceived;

        /// <summary>
        /// Wait for the device that matches the signature to be connected.
        /// </summary>
        /// <param name="vendorId">Vendor Id for the device.</param>
        /// <param name="productId">Product Id for the device.</param>
        /// <param name="usagePage">Usage page for the device.</param>
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

        /// <summary>
        /// Opens a connection to the device and start listening for data.
        /// </summary>
        public static void StartListening()
        {
            _connectedDevice.OpenDevice();
            StartListeningThread();
        }

        /// <summary>
        /// Closes the connection to the device and stops listening for data.
        /// </summary>
        public static void StopListening()
        {
            StopListeningThread();
            _connectedDevice.CloseDevice();
        }

        /// <summary>
        /// Send the byte data to the device.
        /// </summary>
        /// <param name="data">The data to send.</param>
        /// <returns>Whether the data was sent successfully.</returns>
        public static bool SendData(byte[] data)
        {
            return _connectedDevice.Write(data);
        }

        /// <summary>
        /// Sends data to the device and waits for a reply.
        /// </summary>
        /// <param name="data">The data to send.</param>
        /// <returns>The reply data.</returns>
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
