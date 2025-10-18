using System;
using System.Text;
using MelonLoader;

[assembly: MelonInfo(typeof(IMYSHook.Plugin), "IMYSHook-melon", "1.0.6", "IMYSHook")]

namespace IMYSHook;

public class Plugin : MelonMod
{
    public override void OnInitializeMelon()
    {
        if (Console.LargestWindowWidth > 0)
        {
            Console.OutputEncoding = Encoding.UTF8;
        }

        var log = LoggerInstance;
        Global.Log = log;
        log.Msg($"Plugin IMYSHook is loaded!");

        IMYSConfig.Read();
        Translation.InitAsync().Wait();
        Patch.Initialize();
    }

    public class Global
    {
        public static MelonLogger.Instance Log { get; set; }
    }
}