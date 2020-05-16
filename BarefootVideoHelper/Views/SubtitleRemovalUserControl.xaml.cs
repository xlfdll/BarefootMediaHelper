using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Win32;

namespace BarefootVideoHelper
{
    /// <summary>
    /// SubtitleRemovalUserControl.xaml の相互作用ロジック
    /// </summary>
    public partial class SubtitleRemovalUserControl : UserControl
    {
        public SubtitleRemovalUserControl()
        {
            InitializeComponent();
        }

        private void SourceVideoFileNameBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "Supported Formats (*.mp4;*.flv;*.mkv;*.avi)|*.mp4;*.flv;*.mkv;*.avi|All Files (*.*)|*.*",
                FileName = SourceVideoFileNameTextBox.Text
            };

            if (dialog.ShowDialog() == true)
            {
                SourceVideoFileNameTextBox.Text = dialog.FileName;

                String extension = Path.GetExtension(dialog.FileName);

                OutputFileNameTextBox.Text = dialog.FileName.Replace(extension, String.Empty) + "-OUTPUT" + extension;
            }
        }

        private void OutputFileNameBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Supported Formats (*.mp4;*.flv;*.mkv;*.avi)|*.mp4;*.flv;*.mkv;*.avi|All Files (*.*)|*.*",
                FileName = OutputFileNameTextBox.Text
            };

            if (dialog.ShowDialog() == true)
            {
                OutputFileNameTextBox.Text = dialog.FileName;
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(SourceVideoFileNameTextBox.Text))
            {
                MessageBox.Show(App.Current.MainWindow, "Please select a video file as source.", App.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (String.IsNullOrEmpty(OutputFileNameTextBox.Text))
            {
                MessageBox.Show(App.Current.MainWindow, "Please select a file as output.", App.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                try
                {
                    SubtitleRemovalMode mode = (SubtitleRemovalMode)ModeComboBox.SelectedIndex;

                    switch (mode)
                    {
                        case SubtitleRemovalMode.Soft:
                            SubtitleRemovalHelper.ExecuteSoftRemoval
                                (SourceVideoFileNameTextBox.Text,
                                OutputFileNameTextBox.Text);

                            break;
                        case SubtitleRemovalMode.Hard:
                            if (String.IsNullOrEmpty(SubtitleTopLeftXFileNameTextBox.Text)
                                || String.IsNullOrEmpty(SubtitleTopLeftYFileNameTextBox.Text))
                            {
                                MessageBox.Show(App.Current.MainWindow, "Please enter subtitle's top left coordinates.", App.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Warning);

                                return;
                            }
                            else
                            {
                                SubtitleRemovalHelper.ExecuteHardRemoval
                                    (SourceVideoFileNameTextBox.Text,
                                    OutputFileNameTextBox.Text,
                                    new Point
                                    (Convert.ToInt32(SubtitleTopLeftXFileNameTextBox.Text),
                                    Convert.ToInt32(SubtitleTopLeftYFileNameTextBox.Text)));

                                break;
                            }
                        default:
                            throw new ArgumentException("Unsupported mode");
                    }

                    MessageBox.Show(App.Current.MainWindow, "Operation completed.", App.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(App.Current.MainWindow, ex.Message, App.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}