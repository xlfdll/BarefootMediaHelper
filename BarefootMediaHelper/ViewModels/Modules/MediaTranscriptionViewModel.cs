using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

using Microsoft.Win32;

using MahApps.Metro.Controls.Dialogs;

using Xlfdll.Windows.Presentation;

namespace BarefootMediaHelper
{
    public class MediaTranscriptionViewModel : ViewModelBase
    {
        public MediaTranscriptionViewModel(MainViewModel mainViewModel)
        {
            this.MainViewModel = mainViewModel;

            this.SelectedModelIndex = 0;
        }

        public MainViewModel MainViewModel { get; }

        private String _sourceFileName;
        private String _outputFileName;
        private Int32 _selectedModelIndex;

        public String SourceFileName
        {
            get
            {
                return _sourceFileName;
            }
            set
            {
                SetField(ref _sourceFileName, value);

                this.OutputFileName = Path.ChangeExtension(value, ".srt");

                // Force re-evaluate CanExecute on all commands
                // Enable the Start button immediately
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public String OutputFileName
        {
            get => _outputFileName;
            private set => SetField(ref _outputFileName, value);
        }

        public Int32 SelectedModelIndex
        {
            get => _selectedModelIndex;
            set => SetField(ref _selectedModelIndex, value);
        }

        public RelayCommand<Object> BrowseSourceFileCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    OpenFileDialog dialog = new OpenFileDialog()
                    {
                        InitialDirectory = String.IsNullOrEmpty(this.SourceFileName)
                            ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                            : String.Empty,
                        FileName = this.SourceFileName,
                        Filter = "Media Files (*.mp4;*.flv;*.mkv;*.avi;*.wav;*.mp3;*.m4a)|*.mp4;*.flv;*.mkv;*.avi;*.wav;*.mp3;*.m4a|All Files (*.*)|*.*"
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        this.SourceFileName = dialog.FileName;
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
                        InitialDirectory = String.IsNullOrEmpty(this.OutputFileName)
                            ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                            : String.Empty,
                        FileName = this.OutputFileName,
                        Filter = "SubRip Subtitle Files (*.srt)|*.srt|All Files (*.*)|*.*"
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
                        if (!File.Exists(this.SourceFileName))
                        {
                            throw new FileNotFoundException($"File not found: {this.SourceFileName}");
                        }

                        await MediaTranscriptionHelper.ExecuteMediaTranscription
                            (this.SourceFileName,
                            this.OutputFileName,
                            this.SelectedModelIndex);

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
                    return !String.IsNullOrEmpty(this.SourceFileName) && !String.IsNullOrEmpty(this.OutputFileName);
                }
            );
    }
}