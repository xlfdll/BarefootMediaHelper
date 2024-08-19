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
            String commonParameters = $"-vcodec libx264 -preset ultrafast -profile:v high -level:v 4.1 -pix_fmt yuv420p -b:v 2000k -acodec aac -strict -2 -ac 2 -ab 192k -ar 44100 -f {outputFileExtension.Remove(0, 1)} -y ";

            if (is60FPS)
            {
                commonParameters += "-r 60 ";
            }

            StringBuilder sb = new StringBuilder();

            sb.Append($"{(useOpenCL ? "-hwaccel auto" : String.Empty)} -i \"{sourceVideoFileName}\" {(useOpenCL ? "-x264opts opencl" : String.Empty)} ");

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