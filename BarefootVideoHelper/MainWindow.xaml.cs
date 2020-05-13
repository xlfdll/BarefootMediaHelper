using System;
using System.IO;
using System.Windows;

using Microsoft.Win32;

namespace BarefootVideoHelper
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			if (!File.Exists("ffmpeg.exe"))
			{
				MessageBox.Show(this, "Cannot find ffmpeg.exe! Program will exit.", this.Title, MessageBoxButton.OK, MessageBoxImage.Error);

				Application.Current.Shutdown();
			}
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

		private void ConvertButton_Click(object sender, RoutedEventArgs e)
		{
			if (String.IsNullOrEmpty(SourceVideoFileNameTextBox.Text))
			{
				MessageBox.Show(this, "Please select a video file as source.", this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			else if (String.IsNullOrEmpty(OutputFileNameTextBox.Text))
			{
				MessageBox.Show(this, "Please select a file as output.", this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			else
			{
				Helper.ExecuteConversion(SourceVideoFileNameTextBox.Text, SourceSubtitleFileNameTextBox.Text, OutputFileNameTextBox.Text,
					HD60FPSCheckBox.IsChecked == true);

				MessageBox.Show(this, "Operation completed.", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}
	}
}