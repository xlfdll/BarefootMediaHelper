using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;

using Xlfdll.Windows.Presentation;

using BarefootVideoHelper.Helpers;

namespace BarefootVideoHelper
{
    public class VideoDownloadViewModel : ViewModelBase
    {
        public VideoDownloadViewModel(MainViewModel mainViewModel)
        {
            this.MainViewModel = mainViewModel;

            this.SelectedQualityIndex = 0;
            this.OutputDirectoryName = VideoDownloadHelper.GetUserDownloadFolderPath();
            this.DoesSkipSponsor = true;

            this.DownloadRequests = new ObservableCollection<VideoDownloadRequest>();
        }

        public MainViewModel MainViewModel { get; }

        private String _videoURL;
        private Int32 _selectedQualityIndex;
        private Int32 _selectedVideoDownloadRequestIndex;
        private String _outputDirectoryName;
        private Boolean _doesSkipSponsor;
        private String _urlErrorCode;

        public String VideoURL
        {
            get => _videoURL;
            set => SetField(ref _videoURL, value);
        }

        public Int32 SelectedQualityIndex
        {
            get => _selectedQualityIndex;
            set => SetField(ref _selectedQualityIndex, value);
        }

        public Int32 SelectedVideoDownloadRequestIndex
        {
            get => _selectedVideoDownloadRequestIndex;
            set => SetField(ref _selectedVideoDownloadRequestIndex, value);
        }

        public String OutputDirectoryName
        {
            get => _outputDirectoryName;
            set => SetField(ref _outputDirectoryName, value);
        }

        public Boolean DoesSkipSponsor
        {
            get => _doesSkipSponsor;
            set => SetField(ref _doesSkipSponsor, value);
        }

        public String URLErrorCode
        {
            get => _urlErrorCode;
            set => SetField(ref _urlErrorCode, value);
        }

        public ObservableCollection<VideoDownloadRequest> DownloadRequests { get; }

        public RelayCommand<Object> AddToQueueCommand
            => new RelayCommand<Object>
            (
                async delegate
                {
                    this.URLErrorCode = null;

                    Boolean isURLValid = this.CheckVideoURL(this.VideoURL);

                    if (isURLValid)
                    {
                        this.MainViewModel.IsBusy = true;

                        if (!this.DownloadRequests.Any(req => req.URL == this.VideoURL))
                        {
                            VideoDownloadRequest videoDownloadRequest = new VideoDownloadRequest(this.VideoURL);

                            await videoDownloadRequest.RetrieveVideoTitle();

                            this.DownloadRequests.Add(videoDownloadRequest);
                        }
                        else
                        {
                            this.URLErrorCode = "DuplicatedURL";
                        }

                        this.NewInput();

                        this.MainViewModel.IsBusy = false;
                    }
                    else
                    {
                        this.URLErrorCode = "URLNotValid";
                    }
                },
                delegate
                {
                    return !String.IsNullOrEmpty(this.VideoURL);
                }
            );

        public RelayCommand<Object> RemoveSelectedCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    if (this.SelectedVideoDownloadRequestIndex >= 0
                        && this.SelectedVideoDownloadRequestIndex < this.DownloadRequests.Count)
                    {
                        this.DownloadRequests.RemoveAt(this.SelectedVideoDownloadRequestIndex);
                    }
                },
                delegate
                {
                    return this.SelectedVideoDownloadRequestIndex >= 0
                        && this.SelectedVideoDownloadRequestIndex < this.DownloadRequests.Count;
                }
            );

        public RelayCommand<Object> BrowseOutputDirectoryCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    using (FolderBrowserDialog dlg = new FolderBrowserDialog())
                    {
                        dlg.Description = "Select a folder to save videos";
                        dlg.SelectedPath = this.OutputDirectoryName;
                        dlg.ShowNewFolderButton = true;

                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            this.OutputDirectoryName = dlg.SelectedPath;
                        }
                    }
                }
            );

        public void NewInput()
        {
            this.VideoURL = String.Empty;
        }

        private Boolean CheckVideoURL(String videoURL)
        {
            Boolean result = false;

            try
            {
                Uri videoUri = new Uri(this.VideoURL);
                result = VideoDownloadHelper.SupportedURLDomains
                    .Contains(videoUri.Host.Replace("www.", String.Empty));
            }
            catch { }

            return result;
        }
    }
}