using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

using ControlzEx.Theming;

namespace BarefootVideoHelper
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAppMode;
            ThemeManager.Current.SyncTheme();
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Ignore Win32Exception raised by MetroWindow when exiting
            if (e.Exception is Win32Exception)
            {
                e.Handled = true;
            }
            else
            {
                MessageBox.Show(e.Exception.Message, Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static String Log { get; set; }
        public static LogWindow LogWindow { get; set; }
        public static LogViewModel LogViewModel
            => App.LogWindow?.DataContext as LogViewModel;
    }
}