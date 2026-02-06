using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

using Microsoft.Win32;

using Xlfdll.Windows.Presentation;

namespace BarefootMediaHelper
{
    public class NCAdjustmentsViewModel : ViewModelBase
    {
        public NCAdjustmentsViewModel(MainViewModel mainViewModel)
        {
            this.MainViewModel = mainViewModel;

            this.Adjustments = new ObservableCollection<NCAdjustment>();
            this.AdjustmentType = NCAdjustmentType.InsertClip;
        }

        public MainViewModel MainViewModel { get; }

        private String _sourceVideoFileName;
        private NCAdjustmentType _adjustmentType;
        private String _newClipVideoFileName;
        private String _outputVideoFileName;
        private Int32 _selectedAdjustmentIndex;
        private TimeSpan _startPosition;
        private TimeSpan _endPosition;

        public String SourceVideoFileName
        {
            get => _sourceVideoFileName;
            set => SetField(ref _sourceVideoFileName, value);
        }
        public NCAdjustmentType AdjustmentType
        {
            get => _adjustmentType;
            set => SetField(ref _adjustmentType, value);
        }
        public String NewClipVideoFileName
        {
            get => _newClipVideoFileName;
            set => SetField(ref _newClipVideoFileName, value);
        }
        public String OutputVideoFileName
        {
            get => _outputVideoFileName;
            set => SetField(ref _outputVideoFileName, value);
        }
        public Int32 SelectedAdjustmentIndex
        {
            get => _selectedAdjustmentIndex;
            set => SetField(ref _selectedAdjustmentIndex, value);
        }
        public String StartPosition
        {
            get => _startPosition.ToString(TimeSpanFormat);
            set => SetField(ref _startPosition, TimeSpan.Parse(value));
        }
        public String EndPosition
        {
            get => _endPosition.ToString(TimeSpanFormat);
            set => SetField(ref _endPosition, TimeSpan.Parse(value));
        }

        public ObservableCollection<NCAdjustment> Adjustments { get; }

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
        public RelayCommand<Object> BrowseOutputVideoFileCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    SaveFileDialog dialog = new SaveFileDialog()
                    {
                        Filter = "Supported Formats (*.mp4;*.flv;*.mkv;*.avi)|*.mp4;*.flv;*.mkv;*.avi|All Files (*.*)|*.*",
                        InitialDirectory = String.IsNullOrEmpty(this.OutputVideoFileName)
                            ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                            : String.Empty,
                        FileName = this.OutputVideoFileName
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        this.OutputVideoFileName = dialog.FileName;
                    }
                }
            );

        public RelayCommand<Object> BrowseNewClipFileCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    OpenFileDialog dialog = new OpenFileDialog()
                    {
                        Filter = "Supported Formats (*.mp4;*.flv;*.mkv;*.avi)|*.mp4;*.flv;*.mkv;*.avi|All Files (*.*)|*.*",
                        InitialDirectory = String.IsNullOrEmpty(this.NewClipVideoFileName)
                            ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                            : String.Empty,
                        FileName = this.NewClipVideoFileName
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        this.NewClipVideoFileName = dialog.FileName;
                    }
                }
            );
        public RelayCommand<Object> AddAdjustmentCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    switch (this.AdjustmentType)
                    {
                        case NCAdjustmentType.InsertClip:
                            {
                                NCInsertClipAdjustment adjustment = new NCInsertClipAdjustment
                                (
                                    this.AdjustmentType,
                                    TimeSpan.Parse(this.StartPosition),
                                    this.NewClipVideoFileName
                                );

                                this.Adjustments.Add(adjustment);

                                break;
                            }
                        case NCAdjustmentType.RemoveRange:
                            {
                                NCRemoveRangeAdjustment adjustment = new NCRemoveRangeAdjustment
                                (
                                    this.AdjustmentType,
                                    TimeSpan.Parse(this.StartPosition),
                                    TimeSpan.Parse(this.EndPosition)
                                );

                                this.Adjustments.Add(adjustment);

                                break;
                            }
                        default:
                            throw new NotSupportedException("Invalid adjustment type");
                    }
                },
                delegate
                {
                    switch (this.AdjustmentType)
                    {
                        case NCAdjustmentType.InsertClip:
                            return !String.IsNullOrEmpty(this.NewClipVideoFileName);
                        case NCAdjustmentType.RemoveRange:
                            return true;
                        default:
                            return false;
                    }
                }
            );
        public RelayCommand<Object> RemoveAdjustmentCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    this.Adjustments.RemoveAt(this.SelectedAdjustmentIndex);
                },
                delegate
                {
                    return this.Adjustments.Count > 0 && this.SelectedAdjustmentIndex >= 0;
                }
            );
        public RelayCommand<Object> ClearAdjustmentCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    this.SelectedAdjustmentIndex = -1;
                    this.Adjustments.Clear();
                },
                delegate
                {
                    return this.Adjustments.Count > 0;
                }
            );

        public RelayCommand<Object> StartCommand
            => new RelayCommand<Object>
            (
                delegate
                {

                },
                delegate
                {
                    return !String.IsNullOrEmpty(this.SourceVideoFileName)
                        && !String.IsNullOrEmpty(this.OutputVideoFileName)
                        && this.Adjustments.Count > 0;
                }
            );

        private const String TimeSpanFormat = @"h\:mm\:ss";
    }
}