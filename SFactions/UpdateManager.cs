using Newtonsoft.Json;
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
                    Utils.Console_WriteLine("[SFactions] An error occurred while checking for updates.", ConsoleColor.Red);
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

            Utils.Console_WriteLine("[SFactions] Checking for updates...", ConsoleColor.White);

            bool isUpToDate = await IsUpToDate(plugin);

            if (isUpToDate)
            {
                Utils.Console_WriteLine("[SFactions] The plugin is up to date!", ConsoleColor.Green);
            }
            else
            {
                Utils.Console_WriteLine("[SFactions] The plugin is not up to date.\n" +
                                        "Please visit https://github.com/Soof4/SFactions/releases/latest to download the latest version.",
                                        ConsoleColor.Red);
            }
        }
    }
}