using System;
using System.IO;
using System.Reflection;

namespace BarefootMediaHelper
{
    public static class ToolPaths
    {
        public static String ToolsPath
            => Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Tools");
        public static String FFMPEGPath
            => Path.Combine(ToolPaths.ToolsPath, "ffmpeg.exe");
        public static String FFProbePath
            => Path.Combine(ToolPaths.ToolsPath, "ffprobe.exe");
        public static String AVS4x26xPath
            => Path.Combine(ToolPaths.ToolsPath, "avs4x26x.exe");
        public static String MP4BoxPath
            => Path.Combine(ToolPaths.ToolsPath, "mp4box.exe");
        public static String YTDLPPath
            => Path.Combine(ToolPaths.ToolsPath, "yt-dlp.exe");
    }
}