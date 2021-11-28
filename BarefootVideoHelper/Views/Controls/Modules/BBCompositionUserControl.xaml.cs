using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

        private void SourceFileNameTextBox_PreviewDragOver(object sender, DragEventArgs e)
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

            if (files != null && supportedExtensions.Contains(Path.GetExtension(files[0]).ToLowerInvariant()))
            {
                BBCompositionViewModel viewModel = this.DataContext as BBCompositionViewModel;

                if (viewModel != null)
                {
                    viewModel.SourceVideoFileName = files[0];
                }
            }
        }

        private void SourceSubtitleFileNameTextBox_Drop(object sender, DragEventArgs e)
        {
            String[] files = e.Data.GetData(DataFormats.FileDrop) as String[];
            String[] supportedExtensions = new String[] { ".ass", ".srt" };

            if (files != null && supportedExtensions.Contains(Path.GetExtension(files[0]).ToLowerInvariant()))
            {
                BBCompositionViewModel viewModel = this.DataContext as BBCompositionViewModel;

                if (viewModel != null)
                {
                    viewModel.SourceSubtitleFileName = files[0];
                }
            }
        }
    }
}