using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace BarefootVideoHelper
{
	public static class Helper
	{
		public static void ExecuteConversion(String sourceVideoFileName, String sourceSubtitleFileName, String outputFileName, Boolean is60FPS)
		{
			String ffmpegDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			String outputDirectory = Path.GetDirectoryName(outputFileName);
			String tempFileName = Path.Combine(outputDirectory, Path.GetRandomFileName());

			// Pass 1
			StringBuilder sb = new StringBuilder();

			sb.Append($"-i \"{sourceVideoFileName}\" ");

			if (!String.IsNullOrEmpty(sourceSubtitleFileName))
			{
				// Subtitle filename must be escaped twice
				sb.Append($"-vf subtitles=\"{sourceSubtitleFileName.Replace(@"\", @"\\\\").Replace(":", @"\\:")}\" ");
			}

			sb.Append(Helper.CommonParameters);

			if (is60FPS)
			{
				sb.Append("-r 60 ");
			}

			sb.Append("-pass 1 ");
			sb.Append(tempFileName);

			using (Process process = Process.Start(Path.Combine(ffmpegDirectory, "ffmpeg.exe"), sb.ToString()))
			{
				process.WaitForExit();
			}

			// Pass 2
			sb = new StringBuilder();

			sb.Append($"-i \"{tempFileName}\" ");
			sb.Append(Helper.CommonParameters);

			if (is60FPS)
			{
				sb.Append("-r 60 ");
			}

			sb.Append("-pass 2 ");
			sb.Append(outputFileName);

			using (Process process = Process.Start(Path.Combine(ffmpegDirectory, "ffmpeg.exe"), sb.ToString()))
			{
				process.WaitForExit();
			}

			File.Delete(tempFileName);

			foreach (String item in Directory.GetFiles(outputDirectory, "ffmpeg2pass-0.*"))
			{
				File.Delete(item);
			}
		}

		private static readonly String CommonParameters = "-vcodec libx264 -preset veryslow -profile:v high -level:v 4.1 -pix_fmt yuv420p -b:v 1780k -acodec aac -strict -2 -ac 2 -ab 128k -ar 44100 -f flv -y ";
	}
}