using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xlfdll.Diagnostics;

namespace BarefootMediaHelper
{
    public class AudioExtractionHelper
    {
        public static async Task ExecuteAudioExtraction(String sourceFileName, Int32 formatIndex, String outputFolderName)
        {
            Int32 audioTrackCount = 0;

            String probeArguments = $"-select_streams a -show_entries stream=index -of csv=p=0 \"{sourceFileName}\"";

            using (RedirectedProcess process = new RedirectedProcess(ToolPaths.FFProbePath, probeArguments))
            {
                process.BaseProcess.Start();
                process.BaseProcess.WaitForExit();

                String output = process.BaseProcess.StandardOutput.ReadLine();

                while (!String.IsNullOrEmpty(output))
                {
                    audioTrackCount = Int32.Parse(output);

                    output = process.BaseProcess.StandardOutput.ReadLine();
                }
            }

            if (audioTrackCount == 0)
            {
                throw new OperationCanceledException("No audio track found.");
            }

            for (Int32 i = 0; i < audioTrackCount; i++)
            {
                String outputFileName = Path.Combine(outputFolderName,
                    $"{Path.GetFileNameWithoutExtension(sourceFileName)}_Audio_{i}.wav");

                String arguments = $"-i \"{sourceFileName}\" -f wav -vn \"{outputFileName}\"";

                using (RedirectedProcess process = new RedirectedProcess(ToolPaths.FFMPEGPath, arguments))
                {
                    App.LogViewModel.RedirectedProcess = process;

                    await process.StartAsync();
                }
            }
        }
    }
}