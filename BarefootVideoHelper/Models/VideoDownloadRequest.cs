using System;

namespace BarefootVideoHelper
{
    public class VideoDownloadRequest
    {
        public VideoDownloadRequest(String url)
        {
            this.URL = url;
        }

        public String URL { get; }
        public String DisplayText { get; set; }
    }
}