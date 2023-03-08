using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Xlfdll.Diagnostics;

namespace BarefootMediaHelper
{
    public enum SubtitleRemovalMode { Soft, Hard }

    public static class SubtitleRemovalHelper
    {
        public static async Task ExecuteSoftRemoval(String sourceVideoFileName, String outputFileName)
        {
            using (RedirectedProcess process = new RedirectedProcess(ToolPaths.FFMPEGPath,
                    $"-i \"{sourceVideoFileName}\" -map 0 -map -0:s -codec copy \"{outputFileName}\""))
            {
                App.LogViewModel.RedirectedProcess = process;

                await process.StartAsync();
            }
        }

        public static async Task ExecuteHardRemoval
            (String sourceVideoFileName, String outputFileName,
            IEnumerable<SubtitleParameters> subtitleParameters)
        {
            // AviSynth for x26x - Process source file using hard-coded subtitle removal algorithm
            String avsFileName = Path.Combine(Path.GetTempPath(), "HardCodedSubtitleRemoval.avs");
            // For some reason avs4x26x program cannot accept input file name with # character
            String x264OutputFileName = Path.Combine(Path.GetTempPath(),
                Path.GetFileNameWithoutExtension(outputFileName).Replace("#", String.Empty) + ".264");

            File.WriteAllText(avsFileName, SubtitleRemovalHelper.WriteHardCodedSubtitleRemovalAVS
                (sourceVideoFileName, ToolPaths.ToolsPath, subtitleParameters));

            String arguments = $"-o \"{x264OutputFileName}\" \"{avsFileName}\"";

            using (RedirectedProcess process = new RedirectedProcess(ToolPaths.AVS4x26xPath, arguments))
            {
                App.LogViewModel.RedirectedProcess = process;

                await process.StartAsync();
            }

            File.Delete(avsFileName);

            // FFVideoSource produces .ffindex files in the same folder of the source file
            if (File.Exists(sourceVideoFileName + ".ffindex"))
            {
                File.Delete(sourceVideoFileName + ".ffindex");
            }

            // MP4Box - Pack AviSynth output into video stream
            String videoOutputFileName = Path.Combine(Path.GetTempPath(),
                Path.GetFileNameWithoutExtension(outputFileName) + ".v");

            arguments = $"-add \"{x264OutputFileName}\" \"{videoOutputFileName}\"";

            using (RedirectedProcess process = new RedirectedProcess(ToolPaths.MP4BoxPath, arguments))
            {
                App.LogViewModel.RedirectedProcess = process;

                await process.StartAsync();
            }

            File.Delete(x264OutputFileName);

            // FFProbe - Detect audio format of the source
            arguments = $"-v error -select_streams a:0 -show_entries stream=codec_name -of default=nokey=1:noprint_wrappers=1" +
                $" \"{sourceVideoFileName}\"";

            String audioFormat = null;

            using (RedirectedProcess process = new RedirectedProcess(ToolPaths.FFProbePath, arguments))
            {
                await Task.Run(() =>
                {
                    process.Start();

                    audioFormat = process.StandardOutput.ReadToEnd().Trim();

                    process.WaitForExit();
                });
            }

            if (!String.IsNullOrEmpty(audioFormat))
            {
                // FFMPEG - Extract audio stream
                String audioOutputFileName = Path.Combine(Path.GetTempPath(),
                    Path.GetFileNameWithoutExtension(outputFileName) + $".{audioFormat}");

                arguments = $"-i \"{sourceVideoFileName}\" -vn -acodec copy \"{audioOutputFileName}\"";

                using (RedirectedProcess process = new RedirectedProcess(ToolPaths.FFMPEGPath, arguments))
                {
                    App.LogViewModel.RedirectedProcess = process;

                    await process.StartAsync();
                }

                // FFMPEG - Remux audio stream with packed video stream
                arguments = $"-i \"{videoOutputFileName}\" -i \"{audioOutputFileName}\" -map 0:v -map 1:a -c copy -y" +
                    $" \"{outputFileName}\"";

                using (RedirectedProcess process = new RedirectedProcess(ToolPaths.FFMPEGPath, arguments))
                {
                    App.LogViewModel.RedirectedProcess = process;

                    await process.StartAsync();
                }

                File.Delete(videoOutputFileName);
                File.Delete(audioOutputFileName);
            }
            else
            {
                throw new InvalidDataException("Unexpected audio format detection error occurred.");
            }
        }

        private static String WriteHardCodedSubtitleRemovalAVS
            (String sourceVideoFileName, String toolsPath,
            IEnumerable<SubtitleParameters> subtitleParameters)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"LoadPlugin(\"{Path.Combine(toolsPath, @"plugins\VDubFilter.dll")}\")");
            sb.AppendLine($"LoadPlugin(\"{Path.Combine(toolsPath, @"plugins\ffms2.dll")}\")");
            sb.AppendLine($"LoadPlugin(\"{Path.Combine(toolsPath, @"plugins\Threshold.dll")}\")");

            sb.AppendLine($"LoadVirtualDubPlugin(\"{Path.Combine(toolsPath, @"filters\exorcist.vdf")}\", \"Exorcist\", 1)");
            sb.AppendLine($"LoadVirtualDubPlugin(\"{Path.Combine(toolsPath, @"filters\Bright.vdf")}\", \"Brightness_Contrast\", 1)");
            sb.AppendLine($"LoadVirtualDubPlugin(\"{Path.Combine(toolsPath, @"filters\filter_coring.vdf")}\", \"Core\", 1)");

            sb.AppendLine($"Import(\"{Path.Combine(toolsPath, @"filters\dekafka.avsi")}\")");
            sb.AppendLine($"Import(\"{Path.Combine(toolsPath, @"filters\subtitles_removal.avsi")}\")");

            sb.AppendLine($"clip = FFVideoSource(\"{sourceVideoFileName}\")");

            foreach (SubtitleParameters parameters in subtitleParameters)
            {
                Int32 topLeftX = parameters.TopLeftX;
                Int32 topLeftY = parameters.TopLeftY;
                Int32 bottomRightX = parameters.BottomRightX;
                Int32 bottomRightY = parameters.BottomRightY;

                if (parameters.ApplyToAllFrames)
                {
                    sb.AppendLine($"clip = removeSubtitles(clip, {topLeftX}, {topLeftY}, {bottomRightX}, {bottomRightY})");
                }
                else
                {
                    Int32 startFrameIndex = parameters.StartFrameIndex;
                    Int32 endFrameIndex = parameters.EndFrameIndex;

                    sb.AppendLine($"clip = removeSubtitlesOnDuration(clip, {topLeftX}, {topLeftY}, {bottomRightX}, {bottomRightY}, {startFrameIndex},{endFrameIndex})");
                }
            }

            sb.AppendLine("return clip");

            return sb.ToString();
        }
    }
}