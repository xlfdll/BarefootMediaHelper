using System;

using WinForms = System.Windows.Forms;

using Microsoft.Win32;

using Xlfdll.Windows.Presentation;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using System.IO;

namespace BarefootMediaHelper
{
    public class AudioExtractionViewModel : ViewModelBase
    {
        public AudioExtractionViewModel(MainViewModel mainViewModel)
        {
            this.MainViewModel = mainViewModel;

            this.SelectedFormatIndex = 0;
            this.OutputFolderName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        public MainViewModel MainViewModel { get; }

        private String _sourceFileName;
        private Int32 _selectedFormatIndex;
        private String _outputFolderName;

        public String SourceFileName
        {
            get => _sourceFileName;
            set => SetField(ref _sourceFileName, value);
        }
        public Int32 SelectedFormatIndex
        {
            get => _selectedFormatIndex;
            set => SetField(ref _selectedFormatIndex, value);
        }
        public String OutputFolderName
        {
            get => _outputFolderName;
            set => SetField(ref _outputFolderName, value);
        }

        public RelayCommand<Object> BrowseSourceFileCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    OpenFileDialog dialog = new OpenFileDialog()
                    {
                        Filter = "Video Files (*.mp4;*.mkv;*.flv)|*.mp4;*.mkv;*.flv|All Files (*.*)|*.*",
                        InitialDirectory = String.IsNullOrEmpty(this.SourceFileName)
                            ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                            : String.Empty,
                        FileName = this.SourceFileName
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        this.SourceFileName = dialog.FileName;
                    }
                }
            );

        public RelayCommand<Object> BrowseOutputFolderCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    using (WinForms.FolderBrowserDialog dlg = new WinForms.FolderBrowserDialog())
                    {
                        dlg.Description = "Select a folder to save audio files";
                        dlg.SelectedPath = this.OutputFolderName;
                        dlg.ShowNewFolderButton = true;

                        if (dlg.ShowDialog() == WinForms.DialogResult.OK)
                        {
                            this.OutputFolderName = dlg.SelectedPath;
                        }
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

                        await AudioExtractionHelper.ExecuteAudioExtraction
                            (this.SourceFileName,
                            this.SelectedFormatIndex,
                            this.OutputFolderName);

                        await controller.CloseAsync();

                        await this.MainViewModel.DialogCoordinator.ShowMessageAsync
                            (this.MainViewModel, String.Empty, "Operation completed.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(App.Current.MainWindow, ex.Message,
                            App.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);

                        await controller.CloseAsync();
                    }

                    this.MainViewModel.IsBusy = false;
                },
                delegate
                {
                    return !String.IsNullOrEmpty(this.SourceFileName) && !String.IsNullOrEmpty(this.OutputFolderName);
                }
            );
    }
}