using System;
using System.IO;

using Newtonsoft.Json;

using Xlfdll.Windows;
using Xlfdll.Text;

using BarefootMediaHelper.Properties;

namespace BarefootMediaHelper
{
    public class AppSettings
    {
        public String DownloadOutputFolderPath { get; set; }
            = UserFolders.Downloads;

        #region Theme Settings

        // Accent: Light, Dark
        public String ThemeAccent { get; set; } = "Dark";
        // Color:
        // Red, Green, Blue,
        // Purple, Orange, Lime,
        // Emerald, Teal, Cyan, Cobalt,
        // Indigo, Violet, Pink, Magenta,
        // Crimson, Amber, Yellow, Brown,
        // Olive, Steel, Mauve, Taupe, Sienna
        public String ThemeColor { get; set; } = "Pink";
        public Boolean SyncWindowsThemeMode { get; set; } = true;

        #endregion

        public void Save()
        {
            this.Save(Resources.AppSettingsFileName);
        }

        public void Save(String fileName)
        {
            String contents = JsonConvert.SerializeObject(this, Formatting.Indented);

            File.WriteAllText(fileName, contents, AdditionalEncodings.UTF8WithoutBOM);
        }

        public static void Create()
        {
            if (!File.Exists(Resources.AppSettingsFileName))
            {
                AppSettings appSettings = new AppSettings();

                appSettings.Save();
            }
        }

        public static AppSettings Load()
        {
            return AppSettings.Load(Resources.AppSettingsFileName);
        }

        public static AppSettings Load(String fileName)
        {
            String contents = File.ReadAllText(fileName, AdditionalEncodings.UTF8WithoutBOM);

            return JsonConvert.DeserializeObject<AppSettings>(contents);
        }
    }
}