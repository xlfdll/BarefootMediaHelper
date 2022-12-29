using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BarefootVideoHelper.Helpers
{
    public static class VideoDownloadHelper
    {
        [DllImport("shell32",
        CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
        private static extern String SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, UInt32 dwFlags, IntPtr hToken);

        public static String GetUserDownloadFolderPath()
        {
            Guid guid = new Guid("374DE290-123F-4565-9164-39C4925E467B");

            return VideoDownloadHelper.SHGetKnownFolderPath(guid, 0, (IntPtr)0);
        }

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

        public static HashSet<String> SupportedURLDomains
            => new HashSet<String>()
            {
                "youtube.com",
                "youtu.be"
            };

        public static HttpClient HttpClient
            => new HttpClient();
    }
}