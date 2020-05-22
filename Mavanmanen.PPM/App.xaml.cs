using System.Windows;

namespace Mavanmanen.PPM
{
    public partial class App
    {
        private readonly WaitingForDeviceWindow _waitingForDeviceWindow = new WaitingForDeviceWindow();

        protected override void OnStartup(StartupEventArgs e)
        {
            Engine.OnConnected += Engine_OnConnected;
            Engine.OnDisconnected += Engine_OnDisconnected;

            _waitingForDeviceWindow.Show();
            Engine.Start();

            base.OnStartup(e);
        }

        private void Engine_OnConnected(object sender, System.EventArgs e)
        {
            Dispatcher.Invoke(() => MainWindow.Show());
            Dispatcher.Invoke(() => _waitingForDeviceWindow.Hide());
        }

        private void Engine_OnDisconnected(object sender, System.EventArgs e)
        {
            Dispatcher.Invoke(() => MainWindow.Hide());
            Dispatcher.Invoke(() => _waitingForDeviceWindow.Show());
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Engine.Stop();
            base.OnExit(e);
        }
    }
}
