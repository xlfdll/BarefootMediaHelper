using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Xlfdll.Diagnostics;

namespace BarefootVideoHelper
{
    public enum ConversionMode { HD, HD60fps, SD }

    public static class BBCompositionHelper
    {
        public static async Task ExecuteConversion(String sourceVideoFileName, String sourceSubtitleFileName, String outputFileName, Boolean is60FPS)
        {
            String outputDirectory = Path.GetDirectoryName(outputFileName);
            String tempFileName = Path.Combine(outputDirectory, Path.GetRandomFileName());
            String outputFileExtension = Path.GetExtension(outputFileName);
            String commonParameters = $"-vcodec libx264 -preset veryslow -profile:v high -level:v 4.1 -pix_fmt yuv420p -b:v 2000k -acodec aac -strict -2 -ac 2 -ab 192k -ar 44100 -f {outputFileExtension.Remove(0, 1)} -y ";

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
            sb.Append($"\"{tempFileName}\"");

            using (RedirectedProcess process = new RedirectedProcess(ToolPaths.FFMPEGPath, sb.ToString()))
            {
                App.LogViewModel.RedirectedProcess = process;

                await process.StartAsync();
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
            sb.Append($"\"{outputFileName}\"");

            using (RedirectedProcess process = new RedirectedProcess(ToolPaths.FFMPEGPath, sb.ToString()))
            {
                App.LogViewModel.RedirectedProcess = process;

                await process.StartAsync();
            }

            File.Delete(tempFileName);

            foreach (String item in Directory.GetFiles(outputDirectory, "ffmpeg2pass-0.*"))
            {
                File.Delete(item);
            }
        }
    }
}