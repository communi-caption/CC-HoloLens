using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Linq;

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
            request.SetRequestHeader("Ocp-Apim-Subscription-Key", KeyManager.OCRSubscription);
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

            var result = JsonConvert.DeserializeObject<OCRResult>(request.downloadHandler.text);
            var boundingBox = result.Regions[0].Lines[0].Words[0].BoundingBox;
            var uj = boundingBox.Split(',').Select(int.Parse).ToArray();

            var leftTop = new Vector2(uj[0],uj[1]);
            var leftBottom = new Vector2(uj[0],uj[1]+uj[3]);
            var rightTop = new Vector2(uj[0] + uj[2],uj[1]);
            var rightBottom = new Vector2(uj[0] + uj[2],uj[1] + uj[3]);

            var matrix = webcam.WebcamToWorldMatrix;

            Debug.Log(">>>  === >>>");
            Debug.Log(matrix);

            leftTop = matrix * leftTop;
            leftBottom = matrix * leftBottom;
            rightTop = matrix * rightTop;
            rightBottom = matrix * rightBottom;
            FindObjectOfType<OCRTextRenderer>().Render((leftTop + leftBottom + rightTop + rightBottom) / 4, rightTop.x - leftTop.x, leftBottom.y - leftTop.y, 0, result.Regions[0].Lines[0].Words[0].Text);
#endif
            yield return null;
        }
    }
}
