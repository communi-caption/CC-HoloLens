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
        return "tr-TR";
        return AdjustLanguage(NativeLanguageCode);
    }

    public string AdjustedForeignLanguage() {
        return "en-US";
        //return AdjustLanguage(ForeignLanguageCode);
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

    const string KEY = "settings_3";

    private void Awake() {
        var json = PlayerPrefs.GetString(KEY, null);
        if (string.IsNullOrWhiteSpace(json))
            json = JsonConvert.SerializeObject(new Settings());
        Debug.Log("json>>" + json);
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
            if (req.isHttpError || req.isNetworkError) {
                yield return new WaitForSecondsRealtime(3f);
                continue;
            }

        Debug.Log("json>>" + json);
            if (string.IsNullOrWhiteSpace(json)) {
                json = JsonConvert.SerializeObject(new Settings());
            }
            
            PlayerPrefs.SetString(KEY, json);
            PlayerPrefs.Save();

            settings = JsonConvert.DeserializeObject<Settings>(json);

            yield return new WaitForSecondsRealtime(3f);
        }
    }
}
