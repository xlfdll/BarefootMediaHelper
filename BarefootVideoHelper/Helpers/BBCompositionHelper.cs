using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BarefootVideoHelper
{
    public enum ConversionMode { HD, HD60fps, SD }

    public static class BBCompositionHelper
    {
        public static void ExecuteConversion(String sourceVideoFileName, String sourceSubtitleFileName, String outputFileName, Boolean is60FPS)
        {
            String outputDirectory = Path.GetDirectoryName(outputFileName);
            String tempFileName = Path.Combine(outputDirectory, Path.GetRandomFileName());
            String outputFileExtension = Path.GetExtension(outputFileName);
            String commonParameters = $"-vcodec libx264 -preset veryslow -profile:v high -level:v 4.1 -pix_fmt yuv420p -b:v 2000k -acodec aac -strict -2 -ac 2 -ab 192k -ar 44100 -f {outputFileExtension.Remove(0, 1)} - y ";

            // Pass 1
            StringBuilder sb = new StringBuilder();

            sb.Append($"-i \"{sourceVideoFileName}\" ");

            if (!String.IsNullOrEmpty(sourceSubtitleFileName))
            {
                // Subtitle filename must be escaped twice
                sb.Append($"-vf subtitles=\"{sourceSubtitleFileName.Replace(@"\", @"\\\\").Replace(":", @"\\:")}\" ");
            }

            sb.Append(commonParameters);

            if (is60FPS)
            {
                sb.Append("-r 60 ");
            }

            sb.Append("-pass 1 ");
            sb.Append(tempFileName);

            using (Process process = Process.Start(MainHelper.FFMPEGPath, sb.ToString()))
            {
                process.WaitForExit();
            }

            // Pass 2
            sb = new StringBuilder();

            sb.Append($"-i \"{tempFileName}\" ");
            sb.Append(commonParameters);

            if (is60FPS)
            {
                sb.Append("-r 60 ");
            }

            sb.Append("-pass 2 ");
            sb.Append(outputFileName);

            using (Process process = Process.Start(MainHelper.FFMPEGPath, sb.ToString()))
            {
                process.WaitForExit();
            }

            File.Delete(tempFileName);

            foreach (String item in Directory.GetFiles(outputDirectory, "ffmpeg2pass-0.*"))
            {
                File.Delete(item);
            }
        }
    }
}