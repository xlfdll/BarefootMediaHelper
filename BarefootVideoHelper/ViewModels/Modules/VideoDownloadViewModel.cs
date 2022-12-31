using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using WinForms = System.Windows.Forms;

using MahApps.Metro.Controls.Dialogs;

using Xlfdll.Windows;
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
            this.OutputFolderName = UserFolders.Downloads;
            this.DoesSkipSponsor = false;

            this.DownloadRequests = new ObservableCollection<VideoDownloadRequest>();
        }

        public MainViewModel MainViewModel { get; }

        private String _videoURL;
        private Int32 _selectedQualityIndex;
        private Int32 _selectedVideoDownloadRequestIndex;
        private String _outputFolderName;
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

        public String OutputFolderName
        {
            get => _outputFolderName;
            set => SetField(ref _outputFolderName, value);
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
                    this.VideoURL = this.VideoURL.Trim();

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
                        this.DownloadRequests.Clear();

                        this.MainViewModel.IsBusy = false;
                    }
                    else
                    {
                        this.URLErrorCode = "URLNotValid";
                    }
                },
                delegate
                {
                    return !String.IsNullOrEmpty(this.VideoURL?.Trim());
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

        public RelayCommand<Object> BrowseOutputFolderCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    using (WinForms.FolderBrowserDialog dlg = new WinForms.FolderBrowserDialog())
                    {
                        dlg.Description = "Select a folder to save videos";
                        dlg.SelectedPath = this.OutputFolderName;
                        dlg.ShowNewFolderButton = true;

                        if (dlg.ShowDialog() == WinForms.DialogResult.OK)
                        {
                            this.OutputFolderName = dlg.SelectedPath;
                        }
                    }
                }
            );

        public RelayCommand<Object> DownloadCommand
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
                        await VideoDownloadHelper.ExecuteVideoDownload
                            (this.DownloadRequests,
                            this.OutputFolderName,
                            this.SelectedQualityIndex,
                            this.DoesSkipSponsor);

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
                    return this.DownloadRequests.Count > 0;
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