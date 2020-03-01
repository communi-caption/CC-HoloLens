using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class YandexTest : MonoBehaviour
{
    void Start()
    {
        StartCoroutine("Loop");
    }

    private IEnumerator Loop()
    {
        var translate = new YTranslate("trnsl.1.1.20200229T212133Z.e184ed33afc29e8a.334aa1670617dbfd188616640084847d586e1852");

        var sw = new Stopwatch();
        while (true)
        {
            YTranslate.Result result = null;
            Action<YTranslate.Result> action = (r) => {
                result = r;
            };
            sw.Restart();

            yield return translate.translate("hello" + UnityEngine.Random.value, YTranslate.Language.TR, action);
            if (result == null)
                continue;
            sw.Stop();
            GetComponentInChildren<Text>().text = sw.ElapsedMilliseconds + " ms";

            yield return new WaitForSecondsRealtime(1.0f);
        }
    }
}