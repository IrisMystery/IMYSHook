using System;
using System.Text;
// using Il2CppInterop.Runtime.Injection;
using MelonLoader;
// using UnityEngine;

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

        ClassInjector.RegisterTypeInIl2Cpp<PluginBehavior>();
        GameObject melonModObject = new GameObject("ModHost");
        melonModObject.AddComponent<PluginBehavior>();
        UnityEngine.Object.DontDestroyOnLoad(melonModObject);
        
    }

    public class Global
    {
        public static MelonLogger.Instance Log { get; set; }
    }
}