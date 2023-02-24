using System;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Windows;

using WinForms = System.Windows.Forms;

using MahApps.Metro.Controls.Dialogs;

using Xlfdll.Windows;
using Xlfdll.Windows.Presentation;

using BarefootVideoHelper.Helpers;
using System.Web.UI.WebControls;

namespace BarefootVideoHelper
{
    public class MediaDownloadViewModel : ViewModelBase
    {
        public MediaDownloadViewModel(MainViewModel mainViewModel)
        {
            this.MainViewModel = mainViewModel;

            this.DownloadRequests = new ObservableCollection<MediaDownloadRequest>();
            this.DownloadRequestCards = new ObservableCollection<dynamic>();

            this.OutputFolderName = UserFolders.Downloads;
        }

        public MainViewModel MainViewModel { get; }

        public ObservableCollection<MediaDownloadRequest> DownloadRequests { get; }
        public ObservableCollection<dynamic> DownloadRequestCards { get; }

        private String _sourceURL;
        private MediaDownloadSource _currentDownloadSource;
        private Int32 _selectedFormatIndex;
        private Int32 _selectedDownloadRequestIndex;
        private String _outputFolderName;
        private Boolean _doesSkipSponsor;
        private String _urlErrorCode;

        public String SourceURL
        {
            get
            {
                return _sourceURL;
            }
            set
            {
                SetField(ref _sourceURL, value.Trim());

                this.CurrentDownloadSource = null;

                if (String.IsNullOrEmpty(_sourceURL))
                {
                    this.URLErrorCode = null;
                }
                else if (!this.DownloadRequests.Any(req => req.URL == _sourceURL))
                {
                    this.CurrentDownloadSource = MediaDownloadHelper.GetDownloadSource(_sourceURL);

                    this.URLErrorCode = this.CurrentDownloadSource == null ? "URLNotValid" : null;
                }
                else
                {
                    this.URLErrorCode = "DuplicatedURL";
                }
            }
        }

        public Int32 SelectedFormatIndex
        {
            get => _selectedFormatIndex;
            set => SetField(ref _selectedFormatIndex, value);
        }
        public Int32 SelectedDownloadRequestIndex
        {
            get => _selectedDownloadRequestIndex;
            set => SetField(ref _selectedDownloadRequestIndex, value);
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

        public MediaDownloadSource CurrentDownloadSource
        {
            get
            {
                return _currentDownloadSource;
            }
            set
            {
                SetField(ref _currentDownloadSource, value);

                OnPropertyChanged(nameof(this.Formats));

                if (value != null)
                {
                    this.SelectedFormatIndex = 0;
                }
            }
        }
        public String[] Formats
        {
            get => this.CurrentDownloadSource?.FormatNames;
        }

        public RelayCommand<Object> PasteCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    this.SourceURL = Clipboard.GetText();
                }
            );

        public RelayCommand<Object> AddToQueueCommand
            => new RelayCommand<Object>
            (
                async delegate
                {
                    this.MainViewModel.IsBusy = true;

                    MediaDownloadRequest request = new MediaDownloadRequest
                            (this.SourceURL, this.SelectedFormatIndex, this.DoesSkipSponsor);

                    dynamic requestCard = new ExpandoObject();

                    requestCard.Title = await request.RetrieveMediaTitle();
                    requestCard.Source = this.CurrentDownloadSource.Name;
                    requestCard.Format = this.Formats[this.SelectedFormatIndex];

                    this.DownloadRequests.Add(request);
                    this.DownloadRequestCards.Add(requestCard);

                    this.SourceURL = String.Empty;

                    this.MainViewModel.IsBusy = false;
                },
                delegate
                {
                    return !String.IsNullOrEmpty(this.SourceURL)
                    && this.CurrentDownloadSource != null
                    && this.SelectedFormatIndex >= 0
                    && this.SelectedFormatIndex < this.Formats.Length
                    && String.IsNullOrEmpty(this.URLErrorCode);
                }
            );

        public RelayCommand<Object> RemoveSelectedCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    if (this.SelectedDownloadRequestIndex >= 0
                        && this.SelectedDownloadRequestIndex < this.DownloadRequests.Count)
                    {
                        this.DownloadRequests.RemoveAt(this.SelectedDownloadRequestIndex);
                        this.DownloadRequestCards.RemoveAt(this.SelectedDownloadRequestIndex);
                    }
                },
                delegate
                {
                    return this.SelectedDownloadRequestIndex >= 0
                        && this.SelectedDownloadRequestIndex < this.DownloadRequests.Count;
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
                        await MediaDownloadHelper.ExecuteMediaDownload(this.OutputFolderName, this.DownloadRequests);

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
    }
}