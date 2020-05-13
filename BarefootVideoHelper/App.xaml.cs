using System.Windows;
using System.Windows.Threading;

namespace BarefootVideoHelper
{
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App : Application
	{
		private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			MessageBox.Show(e.Exception.Message, Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}
}