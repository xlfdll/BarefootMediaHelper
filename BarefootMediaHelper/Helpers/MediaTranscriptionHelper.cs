using System;
using System.IO;
using System.Threading.Tasks;

using Xlfdll.Diagnostics;

namespace BarefootMediaHelper
{
    public static class MediaTranscriptionHelper
    {
        public static async Task ExecuteMediaTranscription(String sourceFileName, String outputFileName, Int32 languageIndex, Int32 modelIndex)
        {
            modelIndex--;

            if (modelIndex < 0)
            {
                modelIndex = 2;
            }

            String tempPath = Path.GetDirectoryName(Path.GetTempPath());
            String arguments = $"\"{sourceFileName}\""
                + $" --model {ModelNames[modelIndex]}"
                + $" --language {LanguageNames[languageIndex]}"
                + $" --output_dir \"{tempPath}\""
                + " --output_format srt";

            using (RedirectedProcess process = new RedirectedProcess(ToolPaths.WhisperCLIPath, arguments))
            {
                App.LogViewModel.RedirectedProcess = process;

                await process.StartAsync();
            }

            File.Move
                (Path.Combine(tempPath, Path.GetFileNameWithoutExtension(sourceFileName) + ".srt"),
                outputFileName);
        }

        private static String[] ModelNames
            => new String[]
        {
            "tiny",
            "base",
            "small",
            "medium",
            "large"
        };

        private static String[] LanguageNames
            => new String[]
        {
            "English",
            "Japanese",
            "Chinese"
        };
    }
}