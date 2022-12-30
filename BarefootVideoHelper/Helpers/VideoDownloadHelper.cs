using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xlfdll.Diagnostics;

namespace BarefootVideoHelper.Helpers
{
    public static class VideoDownloadHelper
    {
        public static async Task RetrieveVideoTitle(this VideoDownloadRequest request)
        {
            if (request != null)
            {
                String contents = await VideoDownloadHelper.HttpClient.GetStringAsync(request.URL);

                request.DisplayText = Regex.Match
                    (contents,
                    @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
                    RegexOptions.IgnoreCase)
                    .Groups["Title"].Value;
            }
        }

        public static async Task ExecuteVideoDownload
            (IEnumerable<VideoDownloadRequest> downloadRequests,
            String outputFolderName,
            Int32 selectedQualityIndex,
            Boolean attemptToSkipSponsor)
        {
            StringBuilder sb = new StringBuilder();
            VideoDownloadQuality videoDownloadQuality = (VideoDownloadQuality)selectedQualityIndex;

            switch (videoDownloadQuality)
            {
                case VideoDownloadQuality.BestCompatibility:
                    sb.Append($"-f \"bv[ext=mp4]+ba[ext=m4a]\"");
                    break;
                case VideoDownloadQuality.BestQuality:
                    sb.Append($"-f \"bv+ba\"");
                    break;
                default:
                    throw new NotSupportedException("Unsupported video download quality enumerable value.");
            }

            sb.Append($" --paths \"{outputFolderName}\"");

            if (attemptToSkipSponsor)
            {
                sb.Append($" --sponsorblock-remove \"sponsor\"");
            }

            String arguments = sb.ToString();

            foreach (VideoDownloadRequest request in downloadRequests)
            {
                using (RedirectedProcess process = new RedirectedProcess
                    (ToolPaths.YTDLPPath, $"{arguments} {request.URL}"))
                {
                    App.LogViewModel.RedirectedProcess = process;

                    await process.StartAsync();
                }
            }
        }

        public static HashSet<String> SupportedURLDomains
            => new HashSet<String>()
            {
                "youtube.com",
                "youtu.be"
            };

        public static HttpClient HttpClient
            => new HttpClient();
    }

    public enum VideoDownloadQuality
    {
        BestCompatibility,
        BestQuality
    }
}