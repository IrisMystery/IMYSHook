using System;
using System.Diagnostics;
using System.Text;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using Il2CppSystem.IO;

namespace IMYSHook;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    public override void Load()
    {
        Console.OutputEncoding = Encoding.UTF8;

        if (File.Exists("IMYSProxy.exe"))
        {
            if (Process.GetProcessesByName("IMYSProxy").Length == 0)
            {
                var start = new ProcessStartInfo("IMYSProxy.exe")
                {
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal
                };
                Process.Start(start);
            }

            Environment.SetEnvironmentVariable("http_proxy", "http://127.0.0.1:8765",
                EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("https_proxy", "http://127.0.0.1:8765",
                EnvironmentVariableTarget.Process);
        }
        else
        {
            Environment.SetEnvironmentVariable("http_proxy", "http://127.0.0.1:5678",
                EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("https_proxy", "http://127.0.0.1:5678",
                EnvironmentVariableTarget.Process);
        }


        var log = Log;
        Global.Log = log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        IMYSConfig.Read();
        Translation.Init();
        Patch.Initialize();

        AddComponent<PluginBehavior>();
    }

    public class Global
    {
        public static ManualLogSource Log { get; set; }
    }
}