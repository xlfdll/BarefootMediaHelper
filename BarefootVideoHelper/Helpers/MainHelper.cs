using System;
using System.IO;
using System.Reflection;

namespace BarefootVideoHelper
{
    public static class MainHelper
    {
        public static String ToolsPath
            => Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Tools");
        public static String FFMPEGPath
            => Path.Combine(MainHelper.ToolsPath, "ffmpeg.exe");
        public static String FFProbePath
            => Path.Combine(MainHelper.ToolsPath, "ffprobe.exe");
        public static String AVS4x26xPath
            => Path.Combine(MainHelper.ToolsPath, "avs4x26x.exe");
        public static String MP4BoxPath
            => Path.Combine(MainHelper.ToolsPath, "mp4box.exe");
    }
}