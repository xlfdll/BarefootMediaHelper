using System;
using System.Threading.Tasks;

using Xlfdll.Diagnostics;

namespace BarefootMediaHelper
{
    public static class MediaTranscriptionHelper
    {
        public static async Task ExecuteMediaTranscription(String sourceFileName, String outputFileName, Int32 modelIndex)
        {
            modelIndex--;

            if (modelIndex < 0)
            {
                modelIndex = 4;
            }

            String arguments = $"/transcribe \"{sourceFileName}\" \"{outputFileName}\" \"{ModelNames[modelIndex]}\"";

            using (RedirectedProcess process = new RedirectedProcess(ToolPaths.BarefootTranscriberPath, arguments))
            {
                App.LogViewModel.RedirectedProcess = process;

                await process.StartAsync();
            }
        }

        private static String[] ModelNames
            => new String[]
        {
            "tiny.en",
            "tiny",
            "base.en",
            "base",
            "small.en",
            "small",
            "medium.en",
            "medium",
            "large"
        };
    }
}