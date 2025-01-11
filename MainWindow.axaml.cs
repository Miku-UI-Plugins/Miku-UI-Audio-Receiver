using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MsBox.Avalonia;

namespace Miku_UI_Music_Center
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (LyricCb.IsChecked == true)
            {
                if (AudioCb.IsChecked == true)
                {
                    var lyricWindow = new LyricFloatWindow(true);
                    lyricWindow.Show();
                }
                else
                {
                    var lyricWindow = new LyricFloatWindow(false);
                    lyricWindow.Show();
                }
                Close();
                return;
            }

            // If the user only wants to listen to audio, we can just open the audio debug window
            if (AudioCb.IsChecked == true)
            {
                var audioDbgWindow = new AudioDebugWindow();
                audioDbgWindow.Show();
                Close();
                return;
            }
            string msgTitle = StringResources.StringResources.DlgSelectErrorTitleText;
            string msgContent = StringResources.StringResources.DlgSelectErrorSummaryText;
            MessageBoxManager.GetMessageBoxStandard(msgTitle, msgContent, icon: MsBox.Avalonia.Enums.Icon.Error).ShowAsync();
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                BeginMoveDrag(e);
            }
        }
    }
}