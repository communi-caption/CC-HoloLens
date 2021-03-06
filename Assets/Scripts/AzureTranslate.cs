﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TranslationResult
{
    public DetectedLanguage DetectedLanguage { get; set; }
    public TextResult SourceText { get; set; }
    public Translation[] Translations { get; set; }
}

public class DetectedLanguage
{
    public string Language { get; set; }
    public float Score { get; set; }
}

public class TextResult
{
    public string Text { get; set; }
    public string Script { get; set; }
}

public class Translation
{
    public string Text { get; set; }
    public TextResult Transliteration { get; set; }
    public string To { get; set; }
    public Alignment Alignment { get; set; }
    public SentenceLength SentLen { get; set; }
}

public class Alignment
{
    public string Proj { get; set; }
}

public class SentenceLength
{
    public int[] SrcSentLen { get; set; }
    public int[] TransSentLen { get; set; }
}

public class AzureTranslate : MonoBehaviour
{
    void Start()
    {
        StartCoroutine("Loop");
    }

    private IEnumerator Loop()
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        while (true)
        {
            sw.Restart();

            if (true || SettingsController.settings.TranslateLanguage == "0") {
                var fromLang = SettingsController.settings.AdjustedForeignLanguage();
                var toLang = SettingsController.settings.AdjustedNativeLanguage();

                object[] body = new object[] { new { Text = Global.Text1 } };
                var requestBody = JsonConvert.SerializeObject(body);
                string requestUrl = $"https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&from={fromLang}&to={toLang}&profanityAction=Marked&profanityMarker=Tag";
                var request = new UnityWebRequest(requestUrl, "POST");
                request.SetRequestHeader("Ocp-Apim-Subscription-Key", KeyManager.TranslateSubscription);
                byte[] bodyRaw = Encoding.UTF8.GetBytes(requestBody);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();
                //Debug.Log("Status Code: " + request.responseCode);
                //Debug.Log("Text: " + (request.downloadHandler.text));
                //Debug.Log("Time:" + sw.ElapsedMilliseconds);

                try {
                    Global.Text3 = JArray.Parse(request.downloadHandler.text)[0]["translations"][0]["text"]?.ToString();
                }
                catch { }
                yield return new WaitForSecondsRealtime(.25f);
            } else {
                Global.Text3 = Global.Text1;
                yield return null;
            }
        }
    }
}