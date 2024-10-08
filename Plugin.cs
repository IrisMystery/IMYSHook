﻿using System;
using System.Text;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;

namespace IMYSHook;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    public override void Load()
    {
        Console.OutputEncoding = Encoding.UTF8;

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