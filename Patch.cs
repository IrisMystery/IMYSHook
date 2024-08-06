using System.IO;
using System.Text.RegularExpressions;
using BepInEx;
using DMM.OLG.Unity.Engine;
using Hachiroku.Novel;
using Hachiroku.Novel.UI;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace IMYSHook;

public class Patch
{
    private static string currentAdvId;
    public static string fontName = "notosanscjktc";
    public static TMP_FontAsset TMPTranslateFont;

    public static void Initialize()
    {
        Harmony.CreateAndPatchAll(typeof(Patch));
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(NovelRoot), "Start")]
    public static void NovelStart(ref NovelRoot __instance)
    {
        if (!IMYSConfig.TranslationEnabled) return;

        if (TMPTranslateFont == null && File.Exists($"{Paths.PluginPath}/font/{fontName}"))
        {
            var ab = AssetBundle.LoadFromFile($"{Paths.PluginPath}/font/{fontName}");
            TMPTranslateFont = ab.LoadAsset<TMP_FontAsset>(fontName + " SDF");
            ab.Unload(false);
        }

        currentAdvId = __instance.Linker.ScenarioId;

        if (!Translation.chapterDicts.ContainsKey(currentAdvId)) Translation.FetchChapterTranslation(currentAdvId);
        Plugin.Global.Log.LogInfo(currentAdvId);
    }

    // Message
    [HarmonyPrefix]
    [HarmonyPatch(typeof(BurikoParseScript), "_SetMssageCommand")]
    public static void Novel_SetMssageCommand(ref int lineNum, ref string line, ref bool isSelectedCaseArea,
        ref int caseCount)
    {
        if (!IMYSConfig.TranslationEnabled) return;

        if (Translation.chapterDicts.ContainsKey(currentAdvId) && line.Contains("「") && line.EndsWith("」"))
        {
            var idx = line.IndexOf('「');
            var name = line.Substring(0, idx);
            var text = line.Substring(idx);

            var full = "";

            string name_replace;
            if (Translation.nameDicts.TryGetValue(name, out name_replace))
                full = name_replace.IsNullOrWhiteSpace() ? text : name_replace;

            string text_replace;
            if (Translation.chapterDicts[currentAdvId].TryGetValue(text, out text_replace))
            {
                text_replace = text_replace.IsNullOrWhiteSpace() ? text : text_replace;
                text_replace = text_replace.Substring(1, text_replace.Length - 2);
                text_replace = text_replace.Replace("「", "『").Replace("」", "』");
                string final_text = "「" + text_replace + "」";
                full += final_text;
            }

            line = full;
        }
        else
        {
            string text_replace;
            if (Translation.chapterDicts.ContainsKey(currentAdvId) &&
                Translation.chapterDicts[currentAdvId].TryGetValue(line, out text_replace))
            {
                text_replace = text_replace.IsNullOrWhiteSpace() ? line : text_replace;
                text_replace = text_replace.Replace("「", "『").Replace("」", "』");
                line = text_replace;
            }
        }
    }

    // Option
    [HarmonyPrefix]
    [HarmonyPatch(typeof(BurikoParseScript), "_ToParamList")]
    public static void Novel_ToParamList(ref string param)
    {
        if (!IMYSConfig.TranslationEnabled) return;

        var re = new Regex(@"{(.*)}");
        var match = re.Match(param);
        if (match.Success)
            for (var i = 0; i < match.Groups.Count; i++)
            {
                var options = match.Groups[i].Value.Split(",");

                for (var i2 = 0; i2 < options.Length; i2++)
                {
                    string text_replace;
                    if (Translation.chapterDicts.ContainsKey(currentAdvId) && Translation.chapterDicts[currentAdvId]
                            .TryGetValue(options[i2], out text_replace))
                    {
                        var option_tr = text_replace.IsNullOrWhiteSpace() ? options[i2] : text_replace;
                        param = param.Replace(options[i2], option_tr);
                    }
                }
            }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MessageScrollView), "CreateItem")]
    public static void CreateItem(ref MessageScrollViewItem item)
    {
        if (!IMYSConfig.TranslationEnabled) return;

        if (TMPTranslateFont != null)
        {
            item._name.font = TMPTranslateFont;
            item._message.font = TMPTranslateFont;
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ChoicesContent), "SetChoiceButtonText")]
    public static void SetChoiceButtonText(ref ChoicesContent __instance)
    {
        if (!IMYSConfig.TranslationEnabled) return;

        if (TMPTranslateFont != null)
            for (var i = 0; i < __instance.choiceTextList.Length; i++)
                __instance.choiceTextList[i].font = TMPTranslateFont;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TextRoot), "DeleteRuby")]
    public static void DeleteRuby(ref TextRoot __instance)
    {
        if (!IMYSConfig.TranslationEnabled) return;

        if (TMPTranslateFont != null)
        {
            if (__instance.CharaName) __instance.CharaName.font = TMPTranslateFont;
            if (__instance.Message) __instance.Message.font = TMPTranslateFont;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(LoginResponse), "Parse")]
    public static void ParseLoginResp(ref ResponseData res)
    {
        Plugin.Global.Log.LogInfo("Account created at: "+res.contents["created_at"].ToString());
        if (File.Exists($"{Paths.PluginPath}/user.txt"))
            Plugin.Global.Log.LogInfo("Account token: " + res.contents["token"].ToString());
    }
}