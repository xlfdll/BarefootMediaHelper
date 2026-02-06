using System.Linq;
using System;
using System.Windows.Controls;
using System.Windows;
using System.IO;

namespace BarefootMediaHelper
{
    /// <summary>
    /// NCAdjustmentsUserControl.xaml の相互作用ロジック
    /// </summary>
    public partial class NCAdjustmentsUserControl : UserControl
    {
        public NCAdjustmentsUserControl()
        {
            InitializeComponent();
        }

        private void FileNameTextBox_PreviewDragOver(object sender, DragEventArgs e)
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
                NCAdjustmentsViewModel viewModel = this.DataContext as NCAdjustmentsViewModel;

                if (viewModel != null)
                {
                    viewModel.SourceVideoFileName = files[0];
                }
            }
        }

        private void NewClipVideoFileName_Drop(object sender, DragEventArgs e)
        {
            String[] files = e.Data.GetData(DataFormats.FileDrop) as String[];
            String[] supportedExtensions = new String[] { ".mp4", ".flv", ".mkv", ".avi" };

            if (files != null && supportedExtensions.Contains(Path.GetExtension(files[0]).ToLowerInvariant()))
            {
                NCAdjustmentsViewModel viewModel = this.DataContext as NCAdjustmentsViewModel;

                if (viewModel != null)
                {
                    viewModel.NewClipVideoFileName = files[0];
                }
            }
        }
    }
}