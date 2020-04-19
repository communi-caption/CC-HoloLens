using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class Settings {
    public string Mode { get; set; } = "0";
    public string NativeLanguageCode { get; set; } = "tr";
    public string ForeignLanguageCode { get; set; } = "en";
    public string SubtitleTrigger { get; set; } = "0";
    public string TranslateLanguage { get; set; } = "0";

    public string AdjustedNativeLanguage() {
        return AdjustLanguage(NativeLanguageCode);
    }

    public string AdjustedForeignLanguage() {
        return AdjustLanguage(ForeignLanguageCode);
    }

    private static string AdjustLanguage(string code) {
        code = code.ToLower();
        if (code == "en") return "en-US";
        if (code.Contains("-")) return code;
        return code + "-" + code.ToUpperInvariant();
    }
}

public class SettingsController : MonoBehaviour {

    public static Settings settings;

    private void Awake() {
        var json = PlayerPrefs.GetString("settings", null);
        if (json == null)
            json = JsonConvert.SerializeObject(new Settings());
        var obj = JsonConvert.DeserializeObject<Settings>(json);
        settings = obj;
    }

    private void Start() {
        StartCoroutine(Loop());
    }

    private IEnumerator Loop() {
        while (true) {
            var req = UnityWebRequest.Get(Global.HOST + "getSettings");
            yield return req.SendWebRequest();

            var json = req.downloadHandler.text;

            if (string.IsNullOrWhiteSpace(json)) {
                json = JsonConvert.SerializeObject(new Settings());
            }
            PlayerPrefs.SetString("settings", json);
            PlayerPrefs.Save();

            settings = JsonConvert.DeserializeObject<Settings>(json);

            yield return new WaitForSecondsRealtime(3f);
        }
    }
}
