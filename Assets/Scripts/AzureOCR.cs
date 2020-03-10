using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

#if WINDOWS_UWP
using Windows.Graphics.Imaging;
using Windows.Media.MediaProperties;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using HoloLensForCV;
#else
public class SoftwareBitmap
{

}
#endif

public class AzureOCR : MonoBehaviour
{
    HololensCameraUWP webcam;

    void Awake()
    {
        webcam = GetComponent<HololensCameraUWP>();
    }

    void Start()
    {
        StartCoroutine("Deneme");
    }

    private async Task<byte[]> SoftwareBitmapToByteArray(SoftwareBitmap softwareBitmap)
    {
#if WINDOWS_UWP
        byte[] array = null;

        using (var ms = new InMemoryRandomAccessStream())
        {
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, ms);
            encoder.SetSoftwareBitmap(softwareBitmap);

            await encoder.FlushAsync();

            array = new byte[ms.Size];
            await ms.ReadAsync(array.AsBuffer(), (uint)ms.Size, InputStreamOptions.None);
        }
        return array;
#else
        return new byte[0];
#endif
    }

    IEnumerator Deneme() {
        var sw = new System.Diagnostics.Stopwatch();
        while (true)
        {
#if WINDOWS_UWP
            sw.Restart();
            string uri = "https://francecentral.api.cognitive.microsoft.com/vision/v2.0/ocr?language=unk&detectOrientation=true";

            var request = new UnityWebRequest(uri, "POST");
            request.SetRequestHeader("Ocp-Apim-Subscription-Key", "14449e1ac3414b5c894fbdca8c38b911");
            request.SetRequestHeader("Content-Type", "application/octet-stream");

            var softwareBitmap = webcam.GetImage();
            if (softwareBitmap == null) {
                yield return null;
                continue;
            }

            var taskGetByteArray = SoftwareBitmapToByteArray(softwareBitmap);
            while (!taskGetByteArray.IsCompleted)
                yield return null;

            byte[] bitmap;
            try {
                bitmap = taskGetByteArray.Result;
            } catch(Exception e) {
                throw e;
            }

            request.uploadHandler = new UploadHandlerRaw(bitmap);
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            Debug.Log(">>>>> Status Code: " + request.responseCode);
            Debug.Log(">>>>> Text: " + (request.downloadHandler.text));
            Debug.Log(">>>>> Time222: " + sw.ElapsedMilliseconds);
#endif
            yield return null;
        }
    }
}
