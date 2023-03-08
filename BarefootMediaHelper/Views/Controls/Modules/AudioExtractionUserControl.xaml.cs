﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace BarefootMediaHelper
{
    /// <summary>
    /// AudioExtractionUserControl.xaml の相互作用ロジック
    /// </summary>
    public partial class AudioExtractionUserControl : UserControl
    {
        public AudioExtractionUserControl()
        {
            InitializeComponent();
        }

        private void SourceVideoFileNameTextBox_PreviewDragOver(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true; // Only stop handling after a file drop
            }
        }

        private void SourceVideoFileNameTextBox_Drop(object sender, System.Windows.DragEventArgs e)
        {
            String[] files = e.Data.GetData(DataFormats.FileDrop) as String[];

            if (files != null)
            {
                AudioExtractionViewModel viewModel = this.DataContext as AudioExtractionViewModel;

                if (viewModel != null)
                {
                    viewModel.SourceFileName = files[0];
                }
            }
        }
    }
}