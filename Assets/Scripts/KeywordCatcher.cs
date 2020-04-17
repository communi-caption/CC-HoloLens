using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KeywordCatcher : MonoBehaviour {

    private static KeywordCatcher instance;
    
    ////////////////////////////////////
    public const string KEYWORD_CAPTURE_PHOTO = "capture photo";
    public const string KEYWORD_CAPTURE_TEXT = "capture text";
    private static readonly string[] KEYWORDS = new string[] { KEYWORD_CAPTURE_PHOTO, KEYWORD_CAPTURE_TEXT };
    ////////////////////////////////////

    private static Dictionary<string, long> timeCache;
    private static string text;

    public PhotoCapturer photoCapturer;

    private void Awake() {
        timeCache = new Dictionary<string, long>();
        instance = this;
    }

    private void Update() {
        if (text != null) {
            text = text.ToLowerInvariant();
            text = new string(text.Where(c => !char.IsPunctuation(c)).ToArray());

            foreach (var keyword in KEYWORDS) {
                if (KeywordCheck(text, keyword)) {
                    if (instance != null) {
                        text = null;
                        instance.OnKeyword(keyword);
                        break;
                    }
                }
            }
        }
    }

    public static void NotifyRecognize(string text) {
        KeywordCatcher.text = text;
    }

    private static bool KeywordCheck(string text, string keyword) {
        if (text == keyword) return true;
        //if (text.StartsWith(keyword + " ")) return true;
        if (text.EndsWith(" " + keyword)) return true;
        //if (text.Contains(" " + keyword + " ")) return true;
        return false;
    }

    private void OnKeyword(string keyword) {
        if (timeCache.ContainsKey(keyword)) {
            var current = System.DateTime.UtcNow.Ticks;
            var last = timeCache[keyword];
            var delta = current - last;
            if (delta < System.TimeSpan.TicksPerSecond * 2) {
                return;
            } else {
                timeCache[keyword] = current;
            }
        } else {
            timeCache[keyword] = System.DateTime.UtcNow.Ticks;
        }

        switch (keyword) {
            case KEYWORD_CAPTURE_PHOTO:
                OnCapturePhoto(false);
                break;
            case KEYWORD_CAPTURE_TEXT:
                OnCapturePhoto(true);
                break;
            default:
                break;
        }
    }

    private void OnCapturePhoto(bool isText) {
        photoCapturer.Capture(isText);
    }
}
