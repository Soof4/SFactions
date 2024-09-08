using System.Reflection;
using System.Text.RegularExpressions;
namespace SFactions.i18net
{
    public static class LocalizationManager
    {
        private static readonly Regex PairRegex = new Regex(@"\s*(\""[^\""]+\"")\s*:\s*(\""[^\""]+\"")\s*");
        private static string _curLang = "en_US";
        public static void LoadLanguage(string langCode)
        {
            FileStream rfs;
            StreamReader sr;

            try
            {
                rfs = File.Open("i18n/SFactions/" + langCode + ".json", FileMode.Open);
                sr = new StreamReader(rfs);
            }
            catch
            {
                return;
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            var type = assembly.GetType("SFactions.i18net.Localization");

            if (type == null) return;

            while (true)
            {
                string? line = sr.ReadLine();

                if (line == null) break;
                if (!PairRegex.IsMatch(line)) continue;

                string[] pair = line.Split(":");
                string key = pair[0].Trim().Trim('"');
                string value = pair[1].Trim().Trim(',').Trim('"');

                FieldInfo? field = type.GetField(key, BindingFlags.Static | BindingFlags.Public);

                if (field == null) continue;
                field.SetValue(null, value);
            }

            _curLang = langCode;
        }
    }
}
