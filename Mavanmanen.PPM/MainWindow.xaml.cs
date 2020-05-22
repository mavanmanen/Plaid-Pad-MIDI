using System.Windows.Input;

namespace Mavanmanen.PPM
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            DataContext = Engine.CurrentState;
            InitializeComponent();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void CloseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
