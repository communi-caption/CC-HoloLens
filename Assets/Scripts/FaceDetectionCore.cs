using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FaceDetectionCore : MonoBehaviour
{
    public TextMeshPro faceDetected;
    public HoloLensCameraUWP holoLensCameraUWP;

    void Start()
    {
#if WINDOWS_UWP
            FaceDetectionUtils.Initialize();
#endif
        InvokeRepeating("refresh", 5f, 3f);
    }

    void refresh()
    {
#if WINDOWS_UWP
        var image = holoLensCameraUWP.GetImage();
        if (image == null) {
            Debug.Log("image is null");
        } else {
            Debug.Log("image is not null");
            bool isFaceDetected = FaceDetectionUtils.DetectFace(image).Result;
            Debug.Log("buraya girdi");
            if(isFaceDetected)
                faceDetected.text = "Face is detected.";
            else
                faceDetected.text = "NO FACE BITTTCCCCHHHHHH";
        }
#endif
    }
}
