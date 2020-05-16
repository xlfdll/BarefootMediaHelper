using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Win32;

namespace BarefootVideoHelper
{
    /// <summary>
    /// CompositionUserControl.xaml の相互作用ロジック
    /// </summary>
    public partial class BBCompositionUserControl : UserControl
    {
        public BBCompositionUserControl()
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
                OutputFileNameTextBox.Text = dialog.FileName.Replace(Path.GetExtension(dialog.FileName), String.Empty) + "-OUTPUT.flv";
            }
        }

        private void SourceSubtitleFileNameBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "Supported Formats (*.ass;*.srt)|*.ass;*.srt|All Files (*.*)|*.*",
                FileName = SourceSubtitleFileNameTextBox.Text
            };

            if (dialog.ShowDialog() == true)
            {
                SourceSubtitleFileNameTextBox.Text = dialog.FileName;
            }
        }

        private void OutputFileNameBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Supported Format (*.flv)|*.flv|All Files (*.*)|*.*",
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
                    BBCompositionHelper.ExecuteConversion(SourceVideoFileNameTextBox.Text, SourceSubtitleFileNameTextBox.Text, OutputFileNameTextBox.Text,
                                HD60FPSCheckBox.IsChecked == true);

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