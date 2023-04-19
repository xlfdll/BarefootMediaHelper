using System.Linq;
using System;
using System.Windows.Controls;
using System.Windows;
using System.IO;

namespace BarefootMediaHelper
{
    /// <summary>
    /// MediaTranscriptionUserControl.xaml の相互作用ロジック
    /// </summary>
    public partial class MediaTranscriptionUserControl : UserControl
    {
        public MediaTranscriptionUserControl()
        {
            InitializeComponent();
        }

        private void SourceFileNameTextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true; // Only stop handling after a file drop
            }
        }

        private void SourceMediaFileNameTextBox_Drop(object sender, DragEventArgs e)
        {
            String[] files = e.Data.GetData(DataFormats.FileDrop) as String[];
            String[] supportedExtensions = new String[] { ".mp4", ".flv", ".mkv", ".avi", "*.wav", "*.mp3", "*.m4a" };

            if (files != null && supportedExtensions.Contains(Path.GetExtension(files[0]).ToLowerInvariant()))
            {
                MediaTranscriptionViewModel viewModel = this.DataContext as MediaTranscriptionViewModel;

                if (viewModel != null)
                {
                    viewModel.SourceFileName = files[0];
                }
            }
        }
    }
}