using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BarefootMediaHelper
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

            if (files != null && supportedExtensions.Contains(Path.GetExtension(files[0]).ToLowerInvariant()))
            {
                SubtitleRemovalViewModel viewModel = this.DataContext as SubtitleRemovalViewModel;

                if (viewModel != null)
                {
                    viewModel.SourceVideoFileName = files[0];
                }
            }
        }
    }
}