using System;
using System.Text;
using System.Threading.Tasks;

using Xlfdll.Diagnostics;

namespace BarefootMediaHelper
{
    public static class FormatConvertHelper
    {
        public static async Task ExecuteFormatConversion(String sourceFileName, String outputFileName, Boolean noReEncode)
        {
            String commonParameters = $"-y ";

            if (noReEncode)
            {
                commonParameters += "-c copy ";
            }

            // Pass 1
            StringBuilder sb = new StringBuilder();

            sb.Append($"-i \"{sourceFileName}\" ");
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