using System.Windows;
using Miku_UI_Audio_Receiver.Windows;

namespace Miku_UI_Audio_Receiver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            var lyricWindow = new LyricFloatWindow();
            lyricWindow.Show();
            Close();
        }
    }
}