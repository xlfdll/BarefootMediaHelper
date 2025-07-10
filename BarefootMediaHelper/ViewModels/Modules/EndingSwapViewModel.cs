using System;

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

        private const String TimeSpanFormat = @"h\:mm\:ss";
    }
}