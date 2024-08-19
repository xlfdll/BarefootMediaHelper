using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

using Microsoft.Win32;

using MahApps.Metro.Controls.Dialogs;

using Xlfdll.Windows.Presentation;

namespace BarefootMediaHelper
{
    public class BBCompositionViewModel : ViewModelBase
    {
        public BBCompositionViewModel(MainViewModel mainViewModel)
        {
            this.MainViewModel = mainViewModel;
        }

        public MainViewModel MainViewModel { get; }

        private String _sourceVideoFileName;
        private String _sourceSubtitleFileName;
        private String _outputFileName;
        private Boolean _is60FPS;
        private Boolean _useOpenCL;

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

                // Force re-evaluate CanExecute on all commands
                // Enable the Start button immediately
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
        public Boolean Is60FPS
        {
            get => _is60FPS;
            set => SetField(ref _is60FPS, value);
        }
        public Boolean UseOpenCL
        {
            get => _useOpenCL;
            set => SetField(ref _useOpenCL, value);
        }

        public RelayCommand<Object> BrowseSourceVideoFileCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    OpenFileDialog dialog = new OpenFileDialog()
                    {
                        Filter = "Supported Formats (*.mp4;*.flv;*.mkv;*.avi)|*.mp4;*.flv;*.mkv;*.avi|All Files (*.*)|*.*",
                        InitialDirectory = String.IsNullOrEmpty(this.SourceVideoFileName)
                            ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                            : String.Empty,
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
                        InitialDirectory = String.IsNullOrEmpty(this.SourceSubtitleFileName)
                            ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                            : String.Empty,
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
                    SaveFileDialog dialog = new SaveFileDialog()
                    {
                        Filter = "MPEG-4 Part 14 (*.mp4)|*.mp4|All Files (*.*)|*.*",
                        InitialDirectory = String.IsNullOrEmpty(this.OutputFileName)
                            ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                            : String.Empty,
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
                async delegate
                {
                    this.MainViewModel.IsBusy = true;

                    ProgressDialogController controller = await this.MainViewModel.DialogCoordinator.ShowProgressAsync
                        (this.MainViewModel, String.Empty, "Processing...");

                    controller.SetIndeterminate();

                    try
                    {
                        if (!File.Exists(this.SourceVideoFileName))
                        {
                            throw new FileNotFoundException($"File not found: {this.SourceVideoFileName}");
                        }
                        else if (!String.IsNullOrEmpty(this.SourceSubtitleFileName)
                            && !File.Exists(this.SourceSubtitleFileName))
                        {
                            throw new FileNotFoundException($"File not found: {this.SourceSubtitleFileName}");
                        }

                        await BBCompositionHelper.ExecuteConversion
                            (this.SourceVideoFileName,
                            this.SourceSubtitleFileName,
                            this.OutputFileName,
                            this.Is60FPS,
                            this.UseOpenCL);

                        await controller.CloseAsync();

                        await this.MainViewModel.DialogCoordinator.ShowMessageAsync
                            (this.MainViewModel, String.Empty, "Operation completed.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(App.Current.MainWindow, ex.Message, App.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);

                        await controller.CloseAsync();
                    }

                    this.MainViewModel.IsBusy = false;
                },
                delegate
                {
                    return !String.IsNullOrEmpty(this.SourceVideoFileName) && !String.IsNullOrEmpty(this.OutputFileName);
                }
            );
    }
}