using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExceptionTextScript : MonoBehaviour
{
    private Text text;

    void Awake()
    {
        Application.logMessageReceived += Application_logMessageReceived;
        text = GetComponent<Text>();
    }

    private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
        if (type != LogType.Error && type != LogType.Exception)
            return;
        SetText(condition);
    }

    public void SetText(string s)
    {
        text.text += s + System.Environment.NewLine;
    }
}
