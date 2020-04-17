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
using System.Text;

#if WINDOWS_UWP
using Windows.Graphics.Imaging;
using Windows.Media.MediaProperties;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using HoloLensForCV;
#endif

public class PhotoCapturer : MonoBehaviour {

    public Image overlayImage;
    public ExceptionTextScript exceptionTextScript;

    private HololensCameraUWP webcam;

    void Awake() {
        webcam = GetComponent<HololensCameraUWP>();
    }

    private async Task<byte[]> SoftwareBitmapToByteArray(SoftwareBitmap softwareBitmap) {
        byte[] array = null;
#if WINDOWS_UWP
        using (var ms = new InMemoryRandomAccessStream()) {
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, ms);
            encoder.SetSoftwareBitmap(softwareBitmap);
            await encoder.FlushAsync();
            array = new byte[ms.Size];
            await ms.ReadAsync(array.AsBuffer(), (uint)ms.Size, InputStreamOptions.None);
        }
#endif
        return array;
    }

    public void Capture() {
        StartCoroutine("StartCapture");
    }

    private IEnumerator StartCapture() {
#if WINDOWS_UWP
        var softwareBitmap = webcam.GetImage();
        if (softwareBitmap == null) {
            yield break;
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

        GeneralTextAnimator.Spawn("Captured.");
        ShowOnScreen(bitmap, webcam.Width, webcam.Height);
#endif
        yield return null;
    }

    private void ShowOnScreen(byte[] data, int w, int h) {
        StartCoroutine(SendSaveMediaRequest(data));
        //var texture = new Texture2D(w, h);
        //texture.LoadImage(data);
        //overlayImage.sprite = Sprite.Create(texture, new Rect(0, 0, w, h), new Vector2(.5f, .5f));
    }

    private IEnumerator SendSaveMediaRequest(byte[] data) {
        var request = new UnityWebRequest(Global.HOST + "saveMediaMessage", "POST");
        request.SetRequestHeader("content-type", "application/json");

        var json = JsonConvert.SerializeObject(new {
            FileSize = data.Length,
            MediaType = false,
            Data = data
        });

        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();
    }
}
