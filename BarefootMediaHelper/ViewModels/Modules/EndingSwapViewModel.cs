using System;

using Microsoft.Win32;

using Xlfdll.Windows.Presentation;

namespace BarefootMediaHelper
{
    public class EndingSwapViewModel : ViewModelBase
    {
        public EndingSwapViewModel(MainViewModel mainViewModel)
        {
            this.MainViewModel = mainViewModel;

            this.SelectedLogoSwapPositionIndex = 0;

            _endingLogoLength = TimeSpan.FromSeconds(6);
        }

        public MainViewModel MainViewModel { get; }

        private String _sourceVideoFileName;
        private String _newEndingClipVideoFileName;
        private String _outputVideoFileName;
        private TimeSpan _endingLogoPosition;
        private TimeSpan _endingLogoLength;
        private TimeSpan _endingCutStartPosition;
        private TimeSpan _endingCutLength;
        private Int32 _selectedLogoSwapPositionIndex;

        public String SourceVideoFileName
        {
            get => _sourceVideoFileName;
            set => SetField(ref _sourceVideoFileName, value);
        }
        public String NewEndingClipVideoFileName
        {
            get => _newEndingClipVideoFileName;
            set => SetField(ref _newEndingClipVideoFileName, value);
        }
        public String OutputVideoFileName
        {
            get => _outputVideoFileName;
            set => SetField(ref _outputVideoFileName, value);
        }
        public String EndingLogoPosition
        {
            get => _endingLogoPosition.ToString(EndingSwapViewModel.TimeSpanFormat);
            set => SetField(ref _endingLogoPosition, TimeSpan.Parse(value));
        }
        public String EndingLogoLength
        {
            get => _endingLogoLength.ToString(EndingSwapViewModel.TimeSpanFormat);
            set => SetField(ref _endingLogoLength, TimeSpan.Parse(value));
        }
        public String EndingCutStartPosition
        {
            get => _endingCutStartPosition.ToString(EndingSwapViewModel.TimeSpanFormat);
            set => SetField(ref _endingCutStartPosition, TimeSpan.Parse(value));
        }
        public String EndingCutLength
        {
            get => _endingCutLength.ToString(EndingSwapViewModel.TimeSpanFormat);
            set => SetField(ref _endingCutLength, TimeSpan.Parse(value));
        }
        public Int32 SelectedLogoSwapPositionIndex
        {
            get => _selectedLogoSwapPositionIndex;
            set => SetField(ref _selectedLogoSwapPositionIndex, value);
        }

        public TimeSpan EndingLogoPositionTimeSpan => _endingLogoPosition;
        public TimeSpan EndingLogoLengthTimeSpan => _endingLogoLength;
        public TimeSpan EndingCutStartPositionTimeSpan => _endingCutStartPosition;
        public TimeSpan EndingCutLengthTimeSpan => _endingCutLength;

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
        public RelayCommand<Object> BrowseNewEndingClipFileCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    OpenFileDialog dialog = new OpenFileDialog()
                    {
                        Filter = "Supported Formats (*.mp4;*.flv;*.mkv;*.avi)|*.mp4;*.flv;*.mkv;*.avi|All Files (*.*)|*.*",
                        InitialDirectory = String.IsNullOrEmpty(this.NewEndingClipVideoFileName)
                            ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                            : String.Empty,
                        FileName = this.NewEndingClipVideoFileName
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        this.NewEndingClipVideoFileName = dialog.FileName;
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
        public RelayCommand<Object> StartCommand
            => new RelayCommand<Object>
            (
                delegate
                {

                },
                delegate
                {
                    return !String.IsNullOrEmpty(this.SourceVideoFileName)
                        && !String.IsNullOrEmpty(this.NewEndingClipVideoFileName)
                        && !String.IsNullOrEmpty(this.OutputVideoFileName);
                }
            );

        private const String TimeSpanFormat = @"h\:mm\:ss";
    }
}