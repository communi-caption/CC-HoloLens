using System.Collections;
using System;
using System.Xml;
using UnityEngine.Networking;

public class YTranslate
{
    private string apiKey;

    public YTranslate(string apiKey)
    {
        this.apiKey = apiKey;
    }

    public class Result
    {
        public const int NETWORK_ERROR = -1;
        public const int SUCCESS = 200;
        public const int INVALID_API_KEY = 401;
        public const int BLOCKED_API_KEY = 402;
        public const int DAILY_REQUESTS_EXCEEDED = 403;
        public const int DAILY_TEXT_EXCEEDED = 404;
        public const int TEXT_LENGTH_EXCEEDED = 413;
        public const int CANNOT_TRANSLATE = 422;
        public const int UNSUPPORTED_DIRECTION = 501;

        public readonly int status;
        public readonly Language language;
        public readonly string translatedText;

        public Result(int status, Language language, string translatedText)
        {
            this.status = status;
            this.language = language;
            this.translatedText = translatedText;
        }
    }

    public IEnumerator translate(string text, Language to, Action<Result> callback)
    {
        return translate(text, null, to, callback);
    }

    public IEnumerator translate(string text, Language? from, Language to, Action<Result> callback)
    {
        string encodedText = UnityWebRequest.EscapeURL(text);
        string lang = ((from != null ? Enum.GetName(typeof(Language), from) + "-" : "") + Enum.GetName(typeof(Language), to)).ToLower();
        string requestUrl = string.Format("https://translate.yandex.net/api/v1.5/tr/translate?key={0}&text={1}&lang={2}", apiKey, encodedText, lang);
        UnityWebRequest request = UnityWebRequest.Get(requestUrl);
        yield return request.SendWebRequest();
        Result result;
        if (request.isNetworkError)
        {
            result = new Result(Result.NETWORK_ERROR, to, null);
        }
        else
        {
            string responseText = request.downloadHandler.text;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(responseText);

            XmlNode translation = doc.SelectSingleNode("Translation");
            XmlNode error = doc.SelectSingleNode("Error");

            int status;
            string translatedText;

            if (translation != null)
            {
                status = int.Parse(translation.Attributes["code"].Value);
                translatedText = doc.SelectSingleNode("Translation/text").InnerText;
            }
            else
            {
                status = int.Parse(error.Attributes["code"].Value);
                translatedText = null;
            }
            result = new Result(status, to, translatedText);
        }
        callback(result);
    }

    public enum Language
    {
        SQ,
        EN,
        AR,
        HY,
        AZ,
        AF,
        EU,
        BE,
        BG,
        BS,
        CY,
        VI,
        HU,
        HT,
        GL,
        NL,
        EL,
        KA,
        DA,
        HE,
        ID,
        GA,
        IT,
        IS,
        ES,
        KK,
        CA,
        KY,
        ZH,
        KO,
        LA,
        LV,
        LT,
        MG,
        MS,
        MT,
        MK,
        MN,
        DE,
        NO,
        FA,
        PL,
        PT,
        RO,
        RU,
        SR,
        SK,
        SL,
        SW,
        TG,
        TH,
        TL,
        TT,
        TR,
        UZ,
        UK,
        FI,
        FR,
        HR,
        CS,
        SV,
        ET,
        JA
    };
}