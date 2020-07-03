using System.IO;
using System.Windows;

using MahApps.Metro.Controls.Dialogs;

namespace BarefootVideoHelper
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainViewModel(DialogCoordinator.Instance);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(ToolPaths.ToolsPath))
            {
                MessageBox.Show(this, "Cannot find tools folder! Program will exit.", this.Title, MessageBoxButton.OK, MessageBoxImage.Error);

                Application.Current.Shutdown();
            }
            else
            {
                App.LogWindow = new LogWindow()
                {
                    Owner = this,
                    DataContext = (this.DataContext as MainViewModel)?.LogViewModel
                };

                App.LogWindow.Show();
            }
        }
    }
}