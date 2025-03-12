using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Miku_UI_Music_Center.Platform.MacOS;

namespace Miku_UI_Music_Center
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
            if (OperatingSystem.IsMacOS())
            {
                MacStatusBar.Init();
                MacStatusBar.SetText(StringResources.StringResources.ApplicationName);
            }  // Only runs on macOS
        }
    }
}