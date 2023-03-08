using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

using Microsoft.Win32;

using MahApps.Metro.Controls.Dialogs;

using Xlfdll.Windows.Presentation;

namespace BarefootMediaHelper
{
    public class FormatConversionViewModel : ViewModelBase
    {
        public FormatConversionViewModel(MainViewModel mainViewModel)
        {
            this.MainViewModel = mainViewModel;

            this.SelectedModeIndex = 0;
        }

        public MainViewModel MainViewModel { get; }

        private Int32 _selectedModeIndex;
        private String _sourceFileName;
        private String _outputFileName;

        public Int32 SelectedModeIndex
        {
            get => _selectedModeIndex;
            set => SetField(ref _selectedModeIndex, value);
        }

        public String SourceFileName
        {
            get
            {
                return _sourceFileName;
            }
            set
            {
                SetField(ref _sourceFileName, value);

                String extension = Path.GetExtension(_sourceFileName);

                switch (this.SelectedModeIndex)
                {
                    case 0: // FLV => MP4
                        this.OutputFileName = _sourceFileName.Replace(extension, ".mp4");

                        break;
                    default:
                        throw new InvalidOperationException("Invalid convert mode value");
                }

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
                        FileName = this.SourceFileName
                    };

                    switch (this.SelectedModeIndex)
                    {
                        case 0: // FLV => MP4
                            dialog.Filter = "Flash Video (*.flv)|*.flv";

                            break;
                        default:
                            throw new InvalidOperationException("Invalid convert mode value");
                    }

                    dialog.Filter += "|All Files (*.*)|*.*";

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
                        FileName = this.OutputFileName
                    };

                    switch (this.SelectedModeIndex)
                    {
                        case 0: // FLV => MP4
                            dialog.Filter = "MPEG-4 Part 14 (*.mp4)|*.mp4";

                            break;
                        default:
                            throw new InvalidOperationException("Invalid convert mode value");
                    }

                    dialog.Filter += "|All Files (*.*)|*.*";

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

                        await FormatConversionHelper.ExecuteFormatConversion
                            (this.SourceFileName,
                            this.OutputFileName,
                            true);

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