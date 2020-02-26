using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using FaceProcessing;
using UnityEngine.Windows.Speech;

#if WINDOWS_UWP
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
#endif

public class HoloFaceCore : MonoBehaviour
{
    [Tooltip("Shows FPS when Debug mode is enabled.")]
    public Text faceDetectedText;
    public float LocalTrackerConfidenceThreshold = 900.0f;
    public float BackendTrackerConfidenceThreshold = 0.1f;
    public int LocalTrackerNumberOfIters = 3;
    public int nLandmarks = 51;

    HololensCameraUWP webcam;
    LocalFaceTracker localFaceTracker;
    
    int nProcessing = 0;
    float imageProcessingStartTime = 0.0f;
    float elapsedTime = 0.0f;
    float lastTipTextUpdateTime = 0.0f;
    Queue<Action> executeOnMainThread = new Queue<Action>();

    bool showDebug = false;
    bool backendConnected = false;
    

    int frameReportPeriod = 10;
    int frameCounter = 0;

    bool faceDetected;

    void Start()
    {
        webcam = GetComponent<HololensCameraUWP>();
        localFaceTracker = new LocalFaceTracker(LocalTrackerNumberOfIters, LocalTrackerConfidenceThreshold);
    }

#if WINDOWS_UWP
    void Update ()
    {
        if(faceDetected)
            faceDetectedText.text = "Face is detected.";
        else
            faceDetectedText.text = "";

        while (executeOnMainThread.Count > 0)
        {
            Action nextAction = executeOnMainThread.Dequeue();
            nextAction.Invoke();

            if (executeOnMainThread.Count == 0)
            {
                elapsedTime += Time.realtimeSinceStartup - imageProcessingStartTime;
                if (frameCounter % frameReportPeriod == 0)
                {
                    elapsedTime = 0;
                }
                imageProcessingStartTime = Time.realtimeSinceStartup;
                frameCounter++;
            }
        }

        if (nProcessing < 1)
        {
            SoftwareBitmap image = webcam.GetImage();
            if (image != null)
            {
                nProcessing++;
                Task.Run(() => ProcessFrame(image));
            }
        }
    
        //if (Time.time > lastTipTextUpdateTime + 3.0f)
        // faceDetectedText.text = "";

    }

    private async Task ProcessFrame(SoftwareBitmap image)
    {
    
        bool landmarks = await localFaceTracker.GetLandmarks(image);
        faceDetected = landmarks;
        nProcessing--;

    }
#endif



    /*private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        KeywordAction keywordAction;

        if (keywordCollection.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }*/
}
