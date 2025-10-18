using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Il2CppDMM.OLG.Unity.Engine;
using Il2CppHachiroku.Novel;
using Il2CppHachiroku.Novel.UI;
using HarmonyLib;
using Il2CppTMPro;
using UnityEngine;

namespace IMYSHook;

public class Patch
{
    private static string currentAdvId;
    public static string fontName = "notosanscjktc";
    public static TMP_FontAsset TMPTranslateFont;

    public static void Initialize()
    {
        HarmonyLib.Harmony.CreateAndPatchAll(typeof(Patch));
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(NovelRoot), "Start")]
    public static void NovelStart(ref NovelRoot __instance)
    {
        if (!IMYSConfig.TranslationEnabled) return;

        if (TMPTranslateFont == null && File.Exists($"{MelonLoader.Utils.MelonEnvironment.ModsDirectory}/font/{fontName}"))
        {
            var ab = AssetBundle.LoadFromFile($"{MelonLoader.Utils.MelonEnvironment.ModsDirectory}/font/{fontName}");
            TMPTranslateFont = ab.LoadAsset<TMP_FontAsset>(fontName + " SDF");
            ab.Unload(false);
        }

        currentAdvId = __instance.Linker.ScenarioId;

        if (!Translation.chapterDicts.ContainsKey(currentAdvId)) Translation.FetchChapterTranslationAsync(currentAdvId).Wait();
        Plugin.Global.Log.Msg(currentAdvId);
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
                full = string.IsNullOrWhiteSpace(name_replace) ? text : name_replace;

            string text_replace;
            if (Translation.chapterDicts[currentAdvId].TryGetValue(text, out text_replace))
            {
                text_replace = string.IsNullOrWhiteSpace(text_replace) ? text : text_replace;
                text_replace = text_replace.Substring(1, text_replace.Length - 2);
                text_replace = text_replace.Replace("「", "『").Replace("」", "』");
                string final_text = "「" + text_replace + "」";
                full += final_text;
            }
            else
            {
                full += text;
            }

            line = full;
        }
        else
        {
            string text_replace;
            if (Translation.chapterDicts.ContainsKey(currentAdvId) &&
                Translation.chapterDicts[currentAdvId].TryGetValue(line, out text_replace))
            {
                text_replace = string.IsNullOrWhiteSpace(text_replace) ? line : text_replace;
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
                        var option_tr = string.IsNullOrWhiteSpace(text_replace) ? options[i2] : text_replace;
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
        Plugin.Global.Log.Msg("Account created at: " + res.contents["created_at"].ToString());
        if (File.Exists($"{MelonLoader.Utils.MelonEnvironment.ModsDirectory}/user.txt") && string.IsNullOrWhiteSpace(File.ReadAllText($"{MelonLoader.Utils.MelonEnvironment.ModsDirectory}/user.txt", Encoding.UTF8)))
        {
            var token = res.contents["token"].ToString();
            Plugin.Global.Log.Msg("Account token: " + token);
            File.WriteAllText($"{MelonLoader.Utils.MelonEnvironment.ModsDirectory}/user.txt", token);
        }
    }
}