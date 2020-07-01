using System;

using Xlfdll.Diagnostics;
using Xlfdll.Windows.Presentation;
using Xlfdll.Windows.Presentation.Dialogs;

namespace BarefootVideoHelper
{
    public class MainViewModel : BaseViewModel
    {
        public BBCompositionViewModel BBCompositionViewModel { get; }
            = new BBCompositionViewModel();
        public SubtitleRemovalViewModel SubtitleRemovalViewModel { get; }
            = new SubtitleRemovalViewModel();

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