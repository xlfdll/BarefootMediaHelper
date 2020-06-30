using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BarefootVideoHelper
{
    public enum SubtitleRemovalMode { Soft, Hard }

    public static class SubtitleRemovalHelper
    {
        public static void ExecuteSoftRemoval(String sourceVideoFileName, String outputFileName)
        {
            using (Process process = Process.Start(MainHelper.FFMPEGPath,
                    $"-i \"{sourceVideoFileName}\" -map 0 -map -0:s -codec copy \"{outputFileName}\""))
            {
                process.WaitForExit();
            }
        }

        public static void ExecuteHardRemoval
            (String sourceVideoFileName, String outputFileName,
            IEnumerable<SubtitleParameters> subtitleParameters)
        {
            // AviSynth for x26x - Process source file using hard-coded subtitle removal algorithm
            String avsFileName = Path.Combine(Path.GetTempPath(), "HardCodedSubtitleRemoval.avs");
            // For some reason avs4x26x program cannot accept input file name with # character
            String x264OutputFileName = Path.Combine(Path.GetTempPath(),
                Path.GetFileNameWithoutExtension(outputFileName).Replace("#", String.Empty) + ".264");

            File.WriteAllText(avsFileName, SubtitleRemovalHelper.WriteHardCodedSubtitleRemovalAVS
                (sourceVideoFileName, MainHelper.ToolsPath, subtitleParameters));

            String arguments = $"-o \"{x264OutputFileName}\" \"{avsFileName}\"";

            using (Process process = Process.Start(MainHelper.AVS4x26xPath, arguments))
            {
                process.WaitForExit();
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

            using (Process process = Process.Start(MainHelper.MP4BoxPath, arguments))
            {
                process.WaitForExit();
            }

            File.Delete(x264OutputFileName);

            // FFProbe - Detect audio format of the source
            arguments = $"-v error -select_streams a:0 -show_entries stream=codec_name -of default=nokey=1:noprint_wrappers=1" +
                $" \"{sourceVideoFileName}\"";

            ProcessStartInfo audioProbeProcessStartInfo = new ProcessStartInfo
                (MainHelper.FFProbePath, arguments)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            String audioFormat = null;

            using (Process process = new Process()
            {
                StartInfo = audioProbeProcessStartInfo
            })
            {
                process.Start();

                audioFormat = process.StandardOutput.ReadToEnd().Trim();

                process.WaitForExit();
            }

            if (!String.IsNullOrEmpty(audioFormat))
            {
                // FFMPEG - Extract audio stream
                String audioOutputFileName = Path.Combine(Path.GetTempPath(),
                    Path.GetFileNameWithoutExtension(outputFileName) + $".{audioFormat}");

                arguments = $"-i \"{sourceVideoFileName}\" -vn -acodec copy \"{audioOutputFileName}\"";

                using (Process process = Process.Start(MainHelper.FFMPEGPath, arguments))
                {
                    process.WaitForExit();
                }

                // FFMPEG - Remux audio stream with packed video stream
                arguments = $"-i \"{videoOutputFileName}\" -i \"{audioOutputFileName}\" -map 0:v -map 1:a -c copy -y" +
                    $" \"{outputFileName}\"";

                using (Process process = Process.Start(MainHelper.FFMPEGPath, arguments))
                {
                    process.WaitForExit();
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

                if (parameters.ApplyToAllFrames)
                {
                    sb.AppendLine($"clip = removeSubtitles(clip, {topLeftX}, {topLeftY})");
                }
                else
                {
                    Int32 startFrameIndex = parameters.StartFrameIndex;
                    Int32 endFrameIndex = parameters.EndFrameIndex;

                    sb.AppendLine($"clip = removeSubtitlesOnDuration(clip, {topLeftX}, {topLeftY}, {startFrameIndex},{endFrameIndex})");
                }
            }

            sb.AppendLine("return clip");

            return sb.ToString();
        }
    }
}