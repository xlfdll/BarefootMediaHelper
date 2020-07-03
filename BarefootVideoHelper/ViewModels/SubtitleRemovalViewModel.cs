using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

using Microsoft.Win32;

using MahApps.Metro.Controls.Dialogs;

using Xlfdll.Windows.Presentation;

namespace BarefootVideoHelper
{
    public class SubtitleRemovalViewModel : BaseViewModel
    {
        public SubtitleRemovalViewModel(MainViewModel mainViewModel)
        {
            this.MainViewModel = mainViewModel;

            this.SelectedModeIndex = 0;
            this.ApplyToAllFrames = true;
            this.SubtitleParameters = new ObservableCollection<SubtitleParameters>();

            this.NewSubtitleParameterInput();
        }

        public MainViewModel MainViewModel { get; }

        public IDialogCoordinator DialogCoordinator { get; }

        private Int32 _selectedModeIndex;
        private String _sourceVideoFileName;
        private String _outputFileName;

        private Int32 _newSubtitleTopLeftX;
        private Int32 _newSubtitleTopLeftY;
        private Int32 _newSubtitleBottomRightX;
        private Int32 _newSubtitleBottomRightY;
        private Int32 _newSubtitleStartFrameNumber;
        private Int32 _newSubtitleEndFrameNumber;
        private Boolean _applyToAllFrames;
        private Int32 _selectedSubtitleParameterIndex;

        public Int32 SelectedModeIndex
        {
            get => _selectedModeIndex;
            set => SetField(ref _selectedModeIndex, value);
        }

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

        public String OutputFileName
        {
            get => _outputFileName;
            private set => SetField(ref _outputFileName, value);
        }

        public Int32 NewSubtitleTopLeftX
        {
            get => _newSubtitleTopLeftX;
            set => SetField(ref _newSubtitleTopLeftX, value);
        }

        public Int32 NewSubtitleTopLeftY
        {
            get => _newSubtitleTopLeftY;
            set => SetField(ref _newSubtitleTopLeftY, value);
        }

        public Int32 NewSubtitleBottomRightX
        {
            get => _newSubtitleBottomRightX;
            set => SetField(ref _newSubtitleBottomRightX, value);
        }

        public Int32 NewSubtitleBottomRightY
        {
            get => _newSubtitleBottomRightY;
            set => SetField(ref _newSubtitleBottomRightY, value);
        }

        public Int32 NewSubtitleStartFrameNumber
        {
            get => _newSubtitleStartFrameNumber;
            set => SetField(ref _newSubtitleStartFrameNumber, value);
        }

        public Int32 NewSubtitleEndFrameNumber
        {
            get => _newSubtitleEndFrameNumber;
            set => SetField(ref _newSubtitleEndFrameNumber, value);
        }

        public Boolean ApplyToAllFrames
        {
            get => _applyToAllFrames;
            set => SetField(ref _applyToAllFrames, value);
        }

        public Int32 SelectedSubtitleParameterIndex
        {
            get => _selectedSubtitleParameterIndex;
            set => SetField(ref _selectedSubtitleParameterIndex, value);
        }

        public ObservableCollection<SubtitleParameters> SubtitleParameters { get; }

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

        public RelayCommand<Object> AddToParameterListCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    SubtitleParameters subtitleParameters
                        = this.ApplyToAllFrames
                        ? new SubtitleParameters
                            (this.NewSubtitleTopLeftX,
                            this.NewSubtitleTopLeftY,
                            this.NewSubtitleBottomRightX,
                            this.NewSubtitleBottomRightY)
                        : new SubtitleParameters
                            (this.NewSubtitleTopLeftX,
                            this.NewSubtitleTopLeftY,
                            this.NewSubtitleBottomRightX,
                            this.NewSubtitleBottomRightY,
                            this.NewSubtitleStartFrameNumber,
                            this.NewSubtitleEndFrameNumber);

                    this.SubtitleParameters.Add(subtitleParameters);

                    this.NewSubtitleParameterInput();
                }
            );

        public RelayCommand<Object> RemoveSelectedCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    if (this.SelectedSubtitleParameterIndex >= 0
                        && this.SelectedSubtitleParameterIndex < this.SubtitleParameters.Count)
                    {
                        this.SubtitleParameters.RemoveAt(this.SelectedSubtitleParameterIndex);
                    }
                },
                delegate
                {
                    return this.SelectedSubtitleParameterIndex >= 0
                        && this.SelectedSubtitleParameterIndex < this.SubtitleParameters.Count;
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

                        SubtitleRemovalMode mode = (SubtitleRemovalMode)this.SelectedModeIndex;

                        switch (mode)
                        {
                            case SubtitleRemovalMode.Soft:
                                await SubtitleRemovalHelper.ExecuteSoftRemoval
                                    (this.SourceVideoFileName,
                                    this.OutputFileName);

                                break;
                            case SubtitleRemovalMode.Hard:
                                await SubtitleRemovalHelper.ExecuteHardRemoval
                                        (this.SourceVideoFileName,
                                        this.OutputFileName,
                                        this.SubtitleParameters);

                                break;
                            default:
                                throw new ArgumentException("Unsupported mode");
                        }

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
                    Boolean isEnabled = !String.IsNullOrEmpty(this.SourceVideoFileName) && !String.IsNullOrEmpty(this.OutputFileName);

                    SubtitleRemovalMode mode = (SubtitleRemovalMode)this.SelectedModeIndex;

                    if (mode == SubtitleRemovalMode.Hard)
                    {
                        isEnabled &= this.SubtitleParameters.Count > 0;
                    }

                    return isEnabled;
                }
            );

        private void NewSubtitleParameterInput()
        {
            this.NewSubtitleTopLeftX = 0;
            this.NewSubtitleTopLeftY = 0;
            this.NewSubtitleBottomRightX = 0;
            this.NewSubtitleBottomRightY = 0;
            this.NewSubtitleStartFrameNumber = 0;
            this.NewSubtitleEndFrameNumber = 0;
        }
    }
}