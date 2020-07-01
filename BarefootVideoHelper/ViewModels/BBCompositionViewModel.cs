using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

using Microsoft.Win32;

using Xlfdll.Windows.Presentation;

namespace BarefootVideoHelper
{
    public class BBCompositionViewModel : BaseViewModel
    {
        private String _sourceVideoFileName;
        private String _sourceSubtitleFileName;
        private String _outputFileName;
        private Boolean _isHD60FPS;

        public String SourceVideoFileName
        {
            get
            {
                return _sourceVideoFileName;
            }
            set
            {
                SetField(ref _sourceVideoFileName, value);

                String extension = Path.GetExtension(_sourceVideoFileName);

                this.OutputFileName = _sourceVideoFileName.Replace(extension, String.Empty) + "-OUTPUT" + ".mp4";

                CommandManager.InvalidateRequerySuggested();
            }
        }

        public String SourceSubtitleFileName
        {
            get => _sourceSubtitleFileName;
            set => SetField(ref _sourceSubtitleFileName, value);
        }

        public String OutputFileName
        {
            get => _outputFileName;
            private set => SetField(ref _outputFileName, value);
        }

        public Boolean IsHD60FPS
        {
            get => _isHD60FPS;
            set => SetField(ref _isHD60FPS, value);
        }

        public RelayCommand<Object> BrowseSourceVideoFileCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    OpenFileDialog dialog = new OpenFileDialog()
                    {
                        Filter = "Supported Formats (*.mp4;*.flv;*.mkv;*.avi)|*.mp4;*.flv;*.mkv;*.avi|All Files (*.*)|*.*",
                        FileName = this.SourceVideoFileName
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        this.SourceVideoFileName = dialog.FileName;
                    }
                }
            );

        public RelayCommand<Object> BrowseSourceSubtitleFileCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    OpenFileDialog dialog = new OpenFileDialog()
                    {
                        Filter = "Supported Formats (*.ass;*.srt)|*.ass;*.srt|All Files (*.*)|*.*",
                        FileName = this.SourceSubtitleFileName
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        this.SourceSubtitleFileName = dialog.FileName;
                    }
                }
            );

        public RelayCommand<Object> BrowseOutputFileCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    OpenFileDialog dialog = new OpenFileDialog()
                    {
                        Filter = "MPEG-4 Part 14 (*.mp4)|*.mp4|Flash Video (*.flv)|*.flv|All Files (*.*)|*.*",
                        FileName = this.OutputFileName
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        this.OutputFileName = dialog.FileName;
                    }
                }
            );

        public RelayCommand<Object> StartCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    try
                    {
                        BBCompositionHelper.ExecuteConversion
                            (this.SourceVideoFileName,
                            this.SourceSubtitleFileName,
                            this.OutputFileName,
                            this.IsHD60FPS);

                        MessageBox.Show(App.Current.MainWindow, "Operation completed.", App.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(App.Current.MainWindow, ex.Message, App.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                },
                delegate
                {
                    return !String.IsNullOrEmpty(this.SourceVideoFileName) && !String.IsNullOrEmpty(this.OutputFileName);
                }
            );
    }
}