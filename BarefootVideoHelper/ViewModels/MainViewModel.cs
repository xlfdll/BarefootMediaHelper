using System;

using MahApps.Metro.Controls.Dialogs;

using Xlfdll.Diagnostics;
using Xlfdll.Windows.Presentation;
using Xlfdll.Windows.Presentation.Dialogs;

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
        public FormatConvertViewModel FormatConvertViewModel
            => new FormatConvertViewModel(this);
        public LogViewModel LogViewModel
            => new LogViewModel(this);

        private Boolean _isBusy;

        public Boolean IsBusy
        {
            get => _isBusy;
            set => SetField(ref _isBusy, value);
        }

        public RelayCommand<Object> AboutCommand
            => new RelayCommand<Object>
            (
                delegate
                {
                    AboutWindow aboutWindow = new AboutWindow
                        (App.Current.MainWindow,
                        AssemblyMetadata.EntryAssemblyMetadata,
                        new ApplicationPackUri("/Images/Barefoot.png"));

                    aboutWindow.ShowDialog();
                }
            );
    }
}