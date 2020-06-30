namespace BarefootVideoHelper
{
    public class MainViewModel : BaseViewModel
    {
        public BBCompositionViewModel BBCompositionViewModel { get; }
            = new BBCompositionViewModel();
        public SubtitleRemovalViewModel SubtitleRemovalViewModel { get; }
            = new SubtitleRemovalViewModel();
    }
}