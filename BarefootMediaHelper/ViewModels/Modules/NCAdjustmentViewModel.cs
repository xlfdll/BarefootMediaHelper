using System;

using Microsoft.Win32;

using Xlfdll.Windows.Presentation;

namespace BarefootMediaHelper
{
    public class NCAdjustmentsViewModel : ViewModelBase
    {
        public NCAdjustmentsViewModel(MainViewModel mainViewModel)
        {
            this.MainViewModel = mainViewModel;

            this.AdjustmentType = AdjustmentType.InsertClip;
        }

        public MainViewModel MainViewModel { get; }

        private String _sourceVideoFileName;
        private AdjustmentType _adjustmentType;
        private String _newClipVideoFileName;
        private String _outputVideoFileName;

        public String SourceVideoFileName
        {
            get => _sourceVideoFileName;
            set => SetField(ref _sourceVideoFileName, value);
        }
        public AdjustmentType AdjustmentType
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
                        && !String.IsNullOrEmpty(this.NewClipVideoFileName)
                        && !String.IsNullOrEmpty(this.OutputVideoFileName);
                }
            );

        private const String TimeSpanFormat = @"h\:mm\:ss";
    }
}