using System.Linq;
using System;
using System.Windows.Controls;
using System.Windows;
using System.IO;

namespace BarefootMediaHelper
{
    /// <summary>
    /// EndingSwapUserControl.xaml の相互作用ロジック
    /// </summary>
    public partial class EndingSwapUserControl : UserControl
    {
        public EndingSwapUserControl()
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
                EndingSwapViewModel viewModel = this.DataContext as EndingSwapViewModel;

                if (viewModel != null)
                {
                    viewModel.SourceVideoFileName = files[0];
                }
            }
        }

        private void NewEndingClipVideoFileName_Drop(object sender, DragEventArgs e)
        {
            String[] files = e.Data.GetData(DataFormats.FileDrop) as String[];
            String[] supportedExtensions = new String[] { ".mp4", ".flv", ".mkv", ".avi" };

            if (files != null && supportedExtensions.Contains(Path.GetExtension(files[0]).ToLowerInvariant()))
            {
                EndingSwapViewModel viewModel = this.DataContext as EndingSwapViewModel;

                if (viewModel != null)
                {
                    viewModel.NewEndingClipVideoFileName = files[0];
                }
            }
        }
    }
}