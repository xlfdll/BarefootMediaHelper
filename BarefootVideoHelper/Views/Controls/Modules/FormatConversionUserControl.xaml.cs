using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BarefootVideoHelper
{
    /// <summary>
    /// FLV2MP4FormatUserControl.xaml の相互作用ロジック
    /// </summary>
    public partial class FormatConversionUserControl : UserControl
    {
        public FormatConversionUserControl()
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
            String[] supportedExtensions = new String[] { ".flv" };

            if (files != null && supportedExtensions.Contains(Path.GetExtension(files[0]).ToLowerInvariant()))
            {
                FormatConversionViewModel viewModel = this.DataContext as FormatConversionViewModel;

                if (viewModel != null)
                {
                    viewModel.SourceFileName = files[0];
                }
            }
        }
    }
}