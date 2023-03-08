using System;
using System.Text;
using System.Threading.Tasks;

using Xlfdll.Diagnostics;

namespace BarefootMediaHelper
{
    public static class FormatConversionHelper
    {
        public static async Task ExecuteFormatConversion(String sourceFileName, String outputFileName, Boolean noReEncode)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"-i \"{sourceFileName}\" ");
            sb.Append("-y ");

            if (noReEncode)
            {
                sb.Append("-c copy ");
            }

            sb.Append($"\"{outputFileName}\"");

            using (RedirectedProcess process = new RedirectedProcess(ToolPaths.FFMPEGPath, sb.ToString()))
            {
                App.LogViewModel.RedirectedProcess = process;

                await process.StartAsync();
            }
        }
    }
}