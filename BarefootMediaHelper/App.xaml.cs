using System;
using System.Windows;
using System.Windows.Threading;

using ControlzEx.Theming;

namespace BarefootMediaHelper
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppSettings.Create(); // If configuration file does not exist, create it automatically

            App.Settings = AppSettings.Load();

            ThemeManager.Current.ChangeTheme(this, $"{App.Settings.ThemeAccent}.{App.Settings.ThemeColor}");

            if (App.Settings.SyncWindowsThemeMode)
            {
                ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAppMode;
                ThemeManager.Current.SyncTheme();
            }
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // BUG: Ignore exceptions raised by MetroWindow when exiting
            if (e.Exception.Source == "PresentationCore" || e.Exception.Source == "ControlzEx")
            {
                e.Handled = true;
            }
            else
            {
                MessageBox.Show(e.Exception.Message, Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static AppSettings Settings { get; private set; }

        public static String Log { get; set; }
        public static LogWindow LogWindow { get; set; }
        public static LogViewModel LogViewModel
            => App.LogWindow?.DataContext as LogViewModel;
    }
}