using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.FaceAnalysis;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml.Controls;

namespace CC_HoloLens
{
    public class FaceDetectionUtils
    {
        static FaceDetector faceDetector;
        static bool isInitialized;

        public static async Task<bool> Initialize()
        {
            if (isInitialized)
                throw new Exception("Already initialized.");

            isInitialized = true;
            faceDetector = await FaceDetector.CreateAsync();
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
    }
}
