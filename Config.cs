using System.IO;
using System.Text;
using System.Text.Json;

namespace IMYSHook;

public class IMYSConfig
{
    public static double Speed;
    public static int FPS;
    public static bool TranslationEnabled;

    public static bool zhHans;

    public static void Read()
    {
        if (File.Exists("./BepInEx/plugins/config.json"))
        {
            var content = File.ReadAllText("./BepInEx/plugins/config.json", Encoding.UTF8);
            var doc = JsonDocument.Parse(content);
            var config = doc.RootElement;

            var needWrite = false;

            if (config.TryGetProperty("speed", out var sValue))
            {
                Speed = sValue.GetDouble();
            }
            else
            {
                Speed = 0.5;
                needWrite = true;
            }

            if (config.TryGetProperty("fps", out var fValue))
            {
                FPS = fValue.GetInt32();
            }
            else
            {
                FPS = 60;
                needWrite = true;
            }

            if (config.TryGetProperty("translation", out var tValue))
            {
                TranslationEnabled = tValue.GetBoolean();
            }
            else
            {
                TranslationEnabled = true;
                needWrite = true;
            }

            if (config.TryGetProperty("zhHans", out var cValue))
            {
                zhHans = cValue.GetBoolean();
            }
            else
            {
                zhHans = false;
            }
            if (needWrite) WriteJsonFile(Speed, FPS, TranslationEnabled,zhHans);

            Plugin.Global.Log.LogInfo("Current setting:");
            Plugin.Global.Log.LogInfo("Game speed(each step): " + Speed);
            Plugin.Global.Log.LogInfo("FPS: " + FPS);
            Plugin.Global.Log.LogInfo("Translation: " + (TranslationEnabled ? "Enabled" : "Disabled"));
            Plugin.Global.Log.LogInfo("zhHans: " + (zhHans ? "Enabled" : "Disabled"));
        }
        else
        {
            Plugin.Global.Log.LogWarning("config.json not found!!!");
            Plugin.Global.Log.LogWarning("Using default config.");
            Speed = 0.5;
            FPS = 60;
            TranslationEnabled = true;

            // Create default JSON file
            WriteJsonFile(0.5, 60, true,false);
        }
    }

    public static void WriteJsonFile(double speed, int fps, bool enabled, bool zhHans)
    {
        var config = new config
        {
            speed = speed,
            fps = fps,
            translation = enabled,
            zhHans = zhHans
        };

        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("./BepInEx/plugins/config.json", json);
    }

    public class config
    {
        public double speed { get; set; }
        public int fps { get; set; }
        public bool translation { get; set; }
        public bool zhHans { get; set; }
    }
}