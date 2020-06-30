using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

using Microsoft.Win32;

using Xlfdll.Windows.Presentation;

namespace BarefootVideoHelper
{
    public class SubtitleRemovalViewModel : BaseViewModel
    {
        public SubtitleRemovalViewModel()
        {
            this.SelectedModeIndex = 0;
            this.ApplyToAllFrames = true;
            this.SubtitleParameters = new ObservableCollection<SubtitleParameters>();

            this.NewSubtitleParameterInput();
        }

        private Int32 _selectedModeIndex;
        private String _sourceVideoFileName;
        private String _outputFileName;

        private Int32 _newSubtitleTopLeftX;
        private Int32 _newSubtitleTopLeftY;
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
                            this.NewSubtitleTopLeftY)
                        : new SubtitleParameters
                            (this.NewSubtitleTopLeftX,
                            this.NewSubtitleTopLeftY,
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
                delegate
                {
                    if (String.IsNullOrEmpty(this.SourceVideoFileName))
                    {
                        MessageBox.Show(App.Current.MainWindow, "Please select a video file as source.", App.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else if (String.IsNullOrEmpty(this.OutputFileName))
                    {
                        MessageBox.Show(App.Current.MainWindow, "Please select a file as output.", App.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        try
                        {
                            SubtitleRemovalMode mode = (SubtitleRemovalMode)this.SelectedModeIndex;

                            switch (mode)
                            {
                                case SubtitleRemovalMode.Soft:
                                    SubtitleRemovalHelper.ExecuteSoftRemoval
                                        (this.SourceVideoFileName,
                                        this.OutputFileName);

                                    break;
                                case SubtitleRemovalMode.Hard:
                                    SubtitleRemovalHelper.ExecuteHardRemoval
                                            (this.SourceVideoFileName,
                                            this.OutputFileName,
                                            this.SubtitleParameters);

                                    break;
                                default:
                                    throw new ArgumentException("Unsupported mode");
                            }

                            MessageBox.Show(App.Current.MainWindow, "Operation completed.", App.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(App.Current.MainWindow, ex.Message, App.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            );

        private void NewSubtitleParameterInput()
        {
            this.NewSubtitleTopLeftX = 0;
            this.NewSubtitleTopLeftY = 0;
            this.NewSubtitleStartFrameNumber = 0;
            this.NewSubtitleEndFrameNumber = 0;
        }
    }
}