using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateLanguage : MonoBehaviour
{
    void Start()
    {
        StartCoroutine("Loop");
    }

    private IEnumerator Loop()
    {
        var translate = new YTranslate("trnsl.1.1.20200229T212133Z.e184ed33afc29e8a.334aa1670617dbfd188616640084847d586e1852");

        while (true)
        {
            while (string.IsNullOrEmpty(Global.Text1))
                yield return null;

            string old = Global.Text1;

            YTranslate.Result result = null;
            Action<YTranslate.Result> action = (r) => {
                result = r;
            };
            yield return translate.translate(old, YTranslate.Language.TR, action);
            if (result == null)
                continue;
            Debug.Log(result.translatedText);
            Global.Text2 = result.translatedText;
            Global.Text3 = $"{old}\n<color=magenta>{result.translatedText}</color>";
            //Global.Text1 = null;
        }
    }
}
