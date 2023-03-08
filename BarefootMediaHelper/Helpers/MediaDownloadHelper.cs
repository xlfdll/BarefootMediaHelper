using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Xlfdll.Diagnostics;

using BarefootMediaHelper.Properties;

namespace BarefootMediaHelper.Helpers
{
    public static class MediaDownloadHelper
    {
        static MediaDownloadHelper()
        {
            String json = Encoding.UTF8.GetString(Resources.MediaDownloadSources);

            MediaDownloadHelper.SupportedSources = JsonConvert.DeserializeObject<MediaDownloadSource[]>(json);
        }

        public static MediaDownloadSource GetDownloadSource(String url)
        {
            MediaDownloadSource result = null;

            try
            {
                Uri videoUri = new Uri(url);
                String domain = videoUri.Host.Replace("www.", String.Empty);

                result = MediaDownloadHelper.SupportedSources.FirstOrDefault(source => source.Domains.Contains(domain));
            }
            catch { }

            return result;
        }

        public static async Task<String> RetrieveMediaTitle(this MediaDownloadRequest request)
        {
            if (request != null)
            {
                String contents = await MediaDownloadHelper.HttpClient.GetStringAsync(request.URL);

                return Regex.Match
                    (contents,
                    @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>",
                    RegexOptions.IgnoreCase)
                    .Groups["Title"].Value;
            }

            return null;
        }

        public static async Task ExecuteMediaDownload
            (String outputFolderName,
            IEnumerable<MediaDownloadRequest> downloadRequests)
        {
            foreach (MediaDownloadRequest request in downloadRequests)
            {
                MediaDownloadSource source = MediaDownloadHelper.GetDownloadSource(request.URL);

                StringBuilder sb = new StringBuilder();

                sb.Append($" --paths \"{outputFolderName}\"");
                sb.Append($" -f \"{source.FormatParameters[request.FormatIndex]}\"");

                if (request.SkipSponsor)
                {
                    sb.Append($" --sponsorblock-remove \"sponsor\"");
                }

                String arguments = sb.ToString();

                using (RedirectedProcess process = new RedirectedProcess
                    (ToolPaths.YTDLPPath, $"{arguments} {request.URL}"))
                {
                    App.LogViewModel.RedirectedProcess = process;

                    await process.StartAsync();
                }
            }
        }


        public static MediaDownloadSource[] SupportedSources { get; }
        public static HttpClient HttpClient
            => new HttpClient();
    }
}