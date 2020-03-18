using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using System.Threading.Tasks;

public class SpeechToText : MonoBehaviour
{
    public Text outputText;

    private object threadLocker = new object();
    private bool waitingForReco;

    private bool micPermissionGranted = false;

    public async void ButtonClick()
    {
        // Creates an instance of a speech config with specified subscription key and service region.
        // Replace with your own subscription key and service region (e.g., "westus").
        var config = SpeechConfig.FromSubscription(KeyManager.SpeechToTextSubscription, KeyManager.SpeechToTextLocation);
        config.SpeechRecognitionLanguage = "en-US";

        // Creates an instance of AutoDetectSourceLanguageConfig with the 2 source language candidates
        // Currently this feature only supports 2 different language candidates
        // Replace the languages to be the language candidates for your speech. Please see https://docs.microsoft.com/azure/cognitive-services/speech-service/language-support for all supported langauges
        //var autoDetectSourceLanguageConfig = AutoDetectSourceLanguageConfig.FromLanguages(new string[] { "en-US", "es-ES" });

        var stopRecognition = new TaskCompletionSource<int>();

        // Creates a speech recognizer using the auto detect source language config, and the file as audio input.
        // Replace with your own audio file name.
        using (var recognizer = new SpeechRecognizer(config/*, autoDetectSourceLanguageConfig*/))
        {
            // Subscribes to events.
            recognizer.Recognizing += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizingSpeech)
                {
                    OnSourceTextSet($"{e.Result.Text}");
                    // Retrieve the detected language
                    var autoDetectSourceLanguageResult = AutoDetectSourceLanguageResult.FromResult(e.Result);
                    //NotifyUser($"DETECTED: Language={autoDetectSourceLanguageResult.Language}");
                }
            };

            recognizer.Recognized += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedSpeech)
                {
                    OnSourceTextSet($"{e.Result.Text}");
                    // Retrieve the detected language
                    var autoDetectSourceLanguageResult = AutoDetectSourceLanguageResult.FromResult(e.Result);
                    //NotifyUser($"DETECTED: Language={autoDetectSourceLanguageResult.Language}");
                }
                else if (e.Result.Reason == ResultReason.NoMatch)
                {
                    OnSourceTextSet($"NOMATCH: Speech could not be recognized.");
                }
            };

            recognizer.Canceled += (s, e) =>
            {
                OnSourceTextSet($"CANCELED: Reason={e.Reason}");

                if (e.Reason == CancellationReason.Error)
                {
                    OnSourceTextSet($"CANCELED: ErrorCode={e.ErrorCode}");
                    OnSourceTextSet($"CANCELED: ErrorDetails={e.ErrorDetails}");
                    OnSourceTextSet($"CANCELED: Did you update the subscription info?");
                }

                stopRecognition.TrySetResult(0);
            };

            recognizer.SessionStarted += (s, e) =>
            {
                OnSourceTextSet("Session started event.");
            };

            recognizer.SessionStopped += (s, e) =>
            {
                OnSourceTextSet("Session stopped event.");
                OnSourceTextSet("Stop recognition.");
                stopRecognition.TrySetResult(0);
            };

            // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
            await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

            // Waits for completion.
            // Use Task.WaitAny to keep the task rooted.
            Task.WaitAny(new[] { stopRecognition.Task });

            // Stops recognition.
            await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
        }
    }

    private void OnSourceTextSet(string s) {
        Global.Text1 = s;
    }

    void Start()
    {
        Invoke("ButtonClick", 2);

        if (outputText == null)
        {
            UnityEngine.Debug.LogError("outputText property is null! Assign a UI Text element to it.");
        }
        else
        {
            micPermissionGranted = true;
            Global.Text1 = "...";
        }
    }

    void Update()
    {
        lock (threadLocker)
        {
            if (outputText != null)
            {
                outputText.text = Global.Text3;
            }
        }
    }
}