using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if WINDOWS_UWP
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.FaceAnalysis;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml.Controls;
using Windows.Media.FaceAnalysis;
using Windows.Graphics.Imaging;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

public class FaceDetectionUtils : MonoBehaviour
{
#if WINDOWS_UWP
    static FaceDetector faceDetector;

    static bool isInitialized;

    public static async Task<bool> Initialize()
    {
        if (isInitialized)
            throw new Exception("Already initialized.");

        isInitialized = true;

        faceDetector = FaceDetector.CreateAsync().AsTask().Result;

        return true;
    }

    public static async Task<bool> IsFaceDetected(SoftwareBitmap previewFrame)
    {
        var result = await faceDetector.DetectFacesAsync(previewFrame);
        return result.Count > 0;
    }

    public static async Task<MediaCapture> WebcamPreview(CaptureElement captureElement)
    {
        var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
        var device = devices[0];

        var mediaInitSettings = new MediaCaptureInitializationSettings { VideoDeviceId = device.Id };
        MediaCapture mediaCapture = new MediaCapture();
        await mediaCapture.InitializeAsync(mediaInitSettings);

        captureElement.Source = mediaCapture;
        await mediaCapture.StartPreviewAsync();

        return mediaCapture;
    }

    public static async Task<SoftwareBitmap> GetSoftwareBitmapFromWebcam(MediaCapture mediaCapture)
    {
        var previewProperties = mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;
        var videoFrame = new VideoFrame(BitmapPixelFormat.Nv12, (int)previewProperties.Width, (int)previewProperties.Height);

        return (await mediaCapture.GetPreviewFrameAsync(videoFrame)).SoftwareBitmap;
    }

    public static async Task<bool> DetectFace(SoftwareBitmap image)
    {
        Debug.Log("log1");

        byte[] imageData = null;

        if (imageData == null || imageData.Length != image.PixelHeight * image.PixelWidth)
            imageData = new byte[image.PixelHeight * image.PixelWidth];

        Debug.Log("log2");

        unsafe
        {
            fixed (byte* grayPointer = imageData)
            {
                FaceProcessing.ImageProcessing.ColorToGrayscale(image, (Int32)(grayPointer));
            }
        }

        Debug.Log("log3");

        SoftwareBitmap grayImage = SoftwareBitmap.CreateCopyFromBuffer(imageData.AsBuffer(), BitmapPixelFormat.Gray8, image.PixelWidth, image.PixelHeight);         

        Debug.Log("log4");

        IList<DetectedFace> faces = null;

    try {faces = await faceDetector.DetectFacesAsync(grayImage);}
    catch(System.Exception e) {
        Debug.Log(e);
        throw;
    }

        Debug.Log("log5");

        List<BitmapBounds> boundingBoxes = new List<BitmapBounds>();
        for (int i = 0; i < faces.Count; i++)
            boundingBoxes.Add(faces[i].FaceBox);

        Debug.Log("log6");

        return boundingBoxes.Count > 0;
    }
#endif
}

