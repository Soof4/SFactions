using Newtonsoft.Json;
using SFactions.i18net;
using TerrariaApi.Server;

namespace SFactions
{
    public static class UpdateManager
    {
        public static async Task<Version?> RequestLatestVersion()
        {
            string url = "https://api.github.com/repos/Soof4/SFactions/releases/latest";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.TryParseAdd("request"); // Set a user agent header

                try
                {
                    var response = await client.GetStringAsync(url);
                    dynamic? latestRelease = JsonConvert.DeserializeObject<dynamic>(response);

                    if (latestRelease == null) return null;

                    string tag = latestRelease.tag_name;

                    tag = tag.Trim('v');
                    string[] nums = tag.Split('.');

                    Version version = new Version(int.Parse(nums[0]),
                                                  int.Parse(nums[1]),
                                                  int.Parse(nums[2])
                                                  );
                    return version;
                }
                catch
                {
                    Utils.Console_WriteLine(Localization.UpdateManager_ErrorMessage, ConsoleColor.Red);
                }
            }

            return null;
        }

        public static async Task<bool> IsUpToDate(TerrariaPlugin plugin)
        {
            Version? latestVersion = await RequestLatestVersion();
            Version curVersion = plugin.Version;

            return latestVersion != null && curVersion >= latestVersion;
        }

        public static async void CheckUpdateVerbose(TerrariaPlugin? plugin)
        {
            if (plugin == null) return;

            Utils.Console_WriteLine(Localization.UpdateManager_CheckingMessage, ConsoleColor.White);

            bool isUpToDate = await IsUpToDate(plugin);

            if (isUpToDate)
            {
                Utils.Console_WriteLine(Localization.UpdateManager_UpToDate, ConsoleColor.Green);
            }
            else
            {
                Utils.Console_WriteLine(Localization.UpdateManager_NotUpToDate, ConsoleColor.Red);
            }
        }
    }
}