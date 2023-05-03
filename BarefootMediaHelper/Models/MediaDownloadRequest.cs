using System;

namespace BarefootMediaHelper
{
    public class MediaDownloadRequest
    {
        public MediaDownloadRequest
            (String url,
            Int32 formatIndex = 0,
            Boolean skipSponsor = false,
            Boolean noPlaylist = true)
        {
            this.URL = url;
            this.FormatIndex = formatIndex;
            this.SkipSponsor = skipSponsor;
            this.NoPlaylist = noPlaylist;
        }

        public String URL { get; }
        public Int32 FormatIndex { get; set; }
        public Boolean SkipSponsor { get; set; }
        public Boolean NoPlaylist { get; set; }
    }
}