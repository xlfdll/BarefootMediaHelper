using System;
using System.IO;
using System.Linq;
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

        private void SourceVideoFileNameTextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true; // Only stop handling after a file drop
            }
        }

        private void SourceVideoFileNameTextBox_Drop(object sender, DragEventArgs e)
        {
            String[] files = e.Data.GetData(DataFormats.FileDrop) as String[];
            String[] supportedExtensions = new String[] { ".mp4", ".flv", ".mkv", ".avi" };

            if (files != null && supportedExtensions.Contains(Path.GetExtension(files[0])))
            {
                SourceVideoFileNameTextBox.Text = files[0];
            }
        }

        private void SourceVideoFileNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            String extension = Path.GetExtension(SourceVideoFileNameTextBox.Text);

            OutputFileNameTextBox.Text = SourceVideoFileNameTextBox.Text.Replace(extension, String.Empty) + "-OUTPUT" + ".mp4";
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
            }
        }

        private void SourceSubtitleFileNameTextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true; // Only stop handling after a file drop
            }
        }

        private void SourceSubtitleFileNameTextBox_Drop(object sender, DragEventArgs e)
        {
            String[] files = e.Data.GetData(DataFormats.FileDrop) as String[];
            String[] supportedExtensions = new String[] { ".ass", ".srt" };

            if (files != null && supportedExtensions.Contains(Path.GetExtension(files[0])))
            {
                SourceSubtitleFileNameTextBox.Text = files[0];
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
                Filter = "MPEG-4 Part 14 (*.mp4)|*.mp4|Flash Video (*.flv)|*.flv|All Files (*.*)|*.*",
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