using Newtonsoft.Json;
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
        while (true)
        {
            //curl -X POST "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&from=en&to=de&profanityAction=Marked&profanityMarker=Tag" -H "Ocp-Apim-Subscription-Key: <client-secret>" -H "Content-Type: application/json; charset=UTF-8" -d "[{'Text':'This is a freaking good idea.'}]"

            //
            {
                string requestUrl = string.Format("https://beyzalitranslate.cognitiveservices.azure.com/sts/v1.0/issuetoken");
                var request = new UnityWebRequest(requestUrl, "POST");
                request.SetRequestHeader("Ocp-Apim-Subscription-Key", "f8d120b88fc148afac4b202241bd399a");
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(new byte[1]);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();
                Debug.Log("Status Code: " + request.responseCode);
                Debug.Log("Text: " + (request.downloadHandler.text));
            }

            {
                object[] body = new object[] { new { Text = "hello bitches" } };
                var requestBody = JsonConvert.SerializeObject(body);
                string requestUrl = string.Format("https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&from=en&to=de&profanityAction=Marked&profanityMarker=Tag");
                var request = new UnityWebRequest(requestUrl, "POST");
                request.SetRequestHeader("Ocp-Apim-Subscription-Key", "f8d120b88fc148afac4b202241bd399a");
                byte[] bodyRaw = Encoding.UTF8.GetBytes(requestBody);
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();
                Debug.Log("Status Code: " + request.responseCode);
                Debug.Log("Text: " + (request.downloadHandler.text));
            }
           
        }
    }
}