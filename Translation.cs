using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using MelonLoader;

namespace IMYSHook;

public class Translation
{
    public static HttpClient client = new(new HttpClientHandler()
    {
        ServerCertificateCustomValidationCallback = delegate { return true; },
    });
    public static Dictionary<string, string> nameDicts = new();
    public static Dictionary<string, Dictionary<string, string>> chapterDicts = new();

    static Translation()
    {
        var melonBase = MelonBase.RegisteredMelons.FirstOrDefault();
        client.DefaultRequestHeaders.UserAgent.ParseAdd($"{melonBase.Info.Name}/{melonBase.Info.Version}");
    }

    public static async Task InitAsync(CancellationToken cancellationToken = default)
    {
        if (!IMYSConfig.TranslationEnabled) return;

        var response =
            await client.GetAsync(
                "https://translation.lolida.best/download/imys/imys_name/zh_Hant/?format=json", cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = response.Content;

            nameDicts = await responseContent.ReadFromJsonAsync<Dictionary<string, string>>(options: null, cancellationToken);
            Plugin.Global.Log.Msg("[Translator] Character name translation loaded. Total: " + nameDicts.Count);
        }
        else
        {
            Plugin.Global.Log.Warning(
                "[Translator] Character name translation failed to load, character name wouldn't translate.");
        }

        var response2 =
            await client.GetAsync(
                "https://translation.lolida.best/download/imys/imys_subname/zh_Hant/?format=json", cancellationToken);
        if (response2.IsSuccessStatusCode)
        {
            var responseContent2 = response2.Content;

            var subNameDicts = await responseContent2.ReadFromJsonAsync<Dictionary<string, string>>(options: null, cancellationToken);
            subNameDicts.ToList().ForEach(x => nameDicts.Add(x.Key, x.Value));
            Plugin.Global.Log.Msg("[Translator] Rando name translation loaded. Total: " + subNameDicts.Count);
        }
        else
        {
            Plugin.Global.Log.Warning(
                "[Translator] Rando name translation failed to load, rando name wouldn't translate.");
        }
    }

    public static async Task FetchChapterTranslationAsync(string label, CancellationToken cancellationToken = default)
    {
        var response =
            await client.GetAsync("https://translation.lolida.best/download/imys/" + label + "/zh_Hant/?format=json", cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = response.Content;

            chapterDicts[label] = await responseContent.ReadFromJsonAsync<Dictionary<string, string>>(options: null, cancellationToken);
            Plugin.Global.Log.Msg("[Translator] Chapter translation loaded. Total: " + chapterDicts[label].Count);
        }
        else
        {
            chapterDicts.Remove(label);
            Plugin.Global.Log.Warning(
                "[Translator] Chapter translation failed to load, chapter text wouldn't translate.");
        }
    }
}