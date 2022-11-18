using System;
using System.Windows.Shell;

using MahApps.Metro.Controls.Dialogs;

using Xlfdll.Diagnostics;
using Xlfdll.Windows.Presentation;
using Xlfdll.Windows.Presentation.Dialogs;

using BarefootVideoHelper.Properties;

namespace BarefootVideoHelper
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(IDialogCoordinator dialogCoordinator)
        {
            this.DialogCoordinator = dialogCoordinator;
        }

        public IDialogCoordinator DialogCoordinator { get; }

        public BBCompositionViewModel BBCompositionViewModel
            => new BBCompositionViewModel(this);
        public SubtitleRemovalViewModel SubtitleRemovalViewModel
            => new SubtitleRemovalViewModel(this);
        public FormatConversionViewModel FormatConvertViewModel
            => new FormatConversionViewModel(this);
        public LogViewModel LogViewModel
            => new LogViewModel(this);

        private Boolean _isBusy;

        public Boolean IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                SetField(ref _isBusy, value);

                OnPropertyChanged(nameof(this.IsTaskProgressOn));
            }
        }

        public TaskbarItemProgressState IsTaskProgressOn
            => (TaskbarItemProgressState)Convert.ToInt32(_isBusy);

        public RelayCommand<Object> AboutCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    AboutWindow aboutWindow = new AboutWindow
                        (App.Current.MainWindow,
                        AssemblyMetadata.EntryAssemblyMetadata,
                        new ApplicationPackUri("/Images/Barefoot.png"),
                        Resources.ExternalSources);

                    aboutWindow.ShowDialog();
                }
            );
    }
}