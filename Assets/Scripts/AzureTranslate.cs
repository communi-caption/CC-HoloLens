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
        sw.Start();

        while (true)
        {
            //Global.Text3 = Global.Text1;

            sw.Restart();

            object[] body = new object[] { new { Text = Global.Text1 } };
            var requestBody = JsonConvert.SerializeObject(body);
            string requestUrl = string.Format("https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&from=en&to=tr&profanityAction=Marked&profanityMarker=Tag");
            //string requestUrl = string.Format("http://aligungor.org/etuders");
            var request = new UnityWebRequest(requestUrl, "POST");
            request.SetRequestHeader("Ocp-Apim-Subscription-Key", "f8d120b88fc148afac4b202241bd399a");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(requestBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            Debug.Log("Status Code: " + request.responseCode);
            Debug.Log("Text: " + (request.downloadHandler.text));
            Debug.Log("Time:" + sw.ElapsedMilliseconds);

            //GetComponentInChildren<Text>().text = sw.ElapsedMilliseconds + " ms\n" + request.downloadHandler.text;
            Global.Text3 = sw.ElapsedMilliseconds + " ms\n" + request.downloadHandler.text;
            yield return new WaitForSecondsRealtime(.25f);
        }
    }
}