using System;

namespace BarefootMediaHelper
{
    public class MediaDownloadRequest
    {
        public MediaDownloadRequest(String url, Int32 formatIndex, Boolean skipSponsor)
        {
            this.URL = url;
            this.FormatIndex = formatIndex;
            this.SkipSponsor = skipSponsor;
        }

        public String URL { get; }
        public Int32 FormatIndex { get; set; }
        public Boolean SkipSponsor { get; set; }
    }
}