using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Xlfdll.Diagnostics;

namespace BarefootMediaHelper
{
    public static class BBCompositionHelper
    {
        public static async Task ExecuteConversion
            (String sourceVideoFileName,
            String sourceSubtitleFileName,
            String outputFileName,
            Boolean is60FPS,
            Boolean useOpenCL
            )
        {
            String outputDirectory = Path.GetDirectoryName(outputFileName);
            String outputFileExtension = Path.GetExtension(outputFileName);
            String commonParameters = $"-vcodec libx264 -preset slower -profile:v high -pix_fmt yuv420p -crf 12 -acodec aac -ac 2 -ab 320k -ar 48000 -movflags +faststart -f {outputFileExtension.Remove(0, 1)} -y ";

            if (is60FPS)
            {
                commonParameters += "-r 60 ";
            }

            StringBuilder sb = new StringBuilder();

            sb.Append($"{(useOpenCL ? "-hwaccel auto" : String.Empty)} -i \"{sourceVideoFileName}\" {(useOpenCL ? "-x264-params opencl=true" : String.Empty)} ");

            if (!String.IsNullOrEmpty(sourceSubtitleFileName))
            {
                String escapedSubtitleFileName = sourceSubtitleFileName.Replace(@"\", @"\\\\").Replace(":", @"\\:").Replace("[", @"\[").Replace("]", @"\]");

                // Subtitle filename must be escaped twice
                sb.Append($"-vf subtitles=\"{escapedSubtitleFileName}\" ");
            }

            sb.Append(commonParameters);
            sb.Append($"\"{outputFileName}\"");

            using (RedirectedProcess process = new RedirectedProcess(ToolPaths.FFMPEGPath, sb.ToString()))
            {
                App.LogViewModel.RedirectedProcess = process;

                await process.StartAsync();
            }
        }
    }
}