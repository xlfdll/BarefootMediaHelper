using System.Windows.Controls;

namespace BarefootVideoHelper
{
    /// <summary>
    /// LogWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class LogWindow
    {
        public LogWindow()
        {
            InitializeComponent();
        }

        private void LogTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox
                && this.DataContext is LogViewModel logViewModel
                && logViewModel.AutoScroll)
            {
                textBox.ScrollToEnd();
            }
        }
    }
}