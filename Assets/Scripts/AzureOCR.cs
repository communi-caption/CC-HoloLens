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
using UnityEngine.UI;

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

    public Image overlayImage;

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
            string uri = "https://westus.api.cognitive.microsoft.com/vision/v2.0/ocr?language=unk&detectOrientation=true";

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

            ShowOnScreen(bitmap, webcam.Width, webcam.Height);

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

            Debug.Log(">>>>> leftTop: " + leftTop);
            Debug.Log(">>>>> leftBottom: " + leftBottom);
            Debug.Log(">>>>> rightTop: " + rightTop);
            Debug.Log(">>>>> rightBottom: " + rightBottom);

            var webcamToWorldTransform = webcam.WebcamToWorldMatrix;
            var WorldSpaceRayPoint1 = webcamToWorldTransform * (new Vector4(0, 0, 0, 1));

            Debug.Log(">>>  === >>>");
            Debug.Log(webcamToWorldTransform);

            {
                int w = webcam.Width;
                int h = webcam.Height;
                Matrix4x4 normK = webcam.ProjectionMatrix;
                float[] arrayK = new float[9];
                arrayK[0 * 3 + 0] = w * normK.m00 / 2;
                arrayK[1 * 3 + 1] = h * normK.m11 / 2;
                arrayK[0 * 3 + 2] = w * (normK.m02 + 1) / 2;
                arrayK[1 * 3 + 2] = h * (normK.m12 + 1) / 2;
                arrayK[2 * 3 + 2] = 1;
                var K = webcam.ProjectionMatrix;
                K.m00 = w * normK.m00 / 2;
                K.m11 = h * normK.m11 / 2;
                K.m02 = w * (normK.m02 + 1) / 2;
                K.m12 = h * (normK.m12 + 1) / 2;
                K.m22 = 1;

                {
                    var webcamSpacePos = Pix2WebcamSpacePos(K, leftTop.x, leftTop.y);
                    var WorldSpaceRayPoint2 = webcamToWorldTransform * webcamSpacePos;
                    leftTop = WorldSpaceRayPoint1 + 1 * WorldSpaceRayPoint2.normalized;
                }
                {
                    var webcamSpacePos = Pix2WebcamSpacePos(K, leftBottom.x, leftBottom.y);
                    var WorldSpaceRayPoint2 = webcamToWorldTransform * webcamSpacePos;
                    leftBottom = WorldSpaceRayPoint1 + 1 * WorldSpaceRayPoint2.normalized;
                }
                {
                    var webcamSpacePos = Pix2WebcamSpacePos(K, rightTop.x, rightTop.y);
                    var WorldSpaceRayPoint2 = webcamToWorldTransform * webcamSpacePos;
                    rightTop = WorldSpaceRayPoint1 + 1 * WorldSpaceRayPoint2.normalized;
                }
                {
                    var webcamSpacePos = Pix2WebcamSpacePos(K, rightBottom.x, rightBottom.y);
                    var WorldSpaceRayPoint2 = webcamToWorldTransform * webcamSpacePos;
                    rightBottom = WorldSpaceRayPoint1 + 1 * WorldSpaceRayPoint2.normalized;
                }
            }

            //leftTop = webcamToWorldTransform * leftTop;
            //leftBottom = webcamToWorldTransform * leftBottom;
            //rightTop = webcamToWorldTransform * rightTop;
            //rightBottom = webcamToWorldTransform * rightBottom;

            FindObjectOfType<OCRTextRenderer>().Render((leftTop + leftBottom + rightTop + rightBottom) / 4, rightTop.x - leftTop.x, leftBottom.y - leftTop.y, 0, result.Regions[0].Lines[0].Words[0].Text);
#endif
            yield return null;
        }
    }

    private Vector3 Pix2WebcamSpacePos(Matrix4x4 K, float x, float y)
    {
        Vector3 res = new Vector3(0.0f, 0.0f, 0.0f);
        res.z = 1.0f;
        res.x = ((webcam.Width - x) - K.m02) / K.m00;
        res.y = (y - K.m12) / K.m11;
        return res * -1.0f;
    }

    void ShowOnScreen(byte[] data, int w, int h)
    {
        var texture = new Texture2D(w, h);
        texture.LoadImage(data);

        overlayImage.sprite = Sprite.Create(texture, new Rect(0, 0, w, h), new Vector2(.5f, .5f));
    }
}
