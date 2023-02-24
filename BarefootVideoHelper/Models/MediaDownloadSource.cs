using System;

namespace BarefootVideoHelper
{
    public class MediaDownloadSource
    {
        public String Name { get; set; }
        public MediaDownloadSourceType Type { get; set; }
        public String[] Domains { get; set; }
        public String[] FormatNames { get; set; }
        public String[] FormatParameters { get; set; }
    }

    public enum MediaDownloadSourceType
    {
        Video,
        Audio
    }
}