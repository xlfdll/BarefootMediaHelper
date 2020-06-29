using System.IO;
using System.Windows;

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
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(MainHelper.ToolsPath))
			{
				MessageBox.Show(this, "Cannot find tools folder! Program will exit.", this.Title, MessageBoxButton.OK, MessageBoxImage.Error);

				Application.Current.Shutdown();

			}
		}
	}
}