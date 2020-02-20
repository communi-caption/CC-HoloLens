using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.FaceAnalysis;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CC_HoloLens
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            var device = devices[0];

            var mediaInitSettings = new MediaCaptureInitializationSettings { VideoDeviceId = device.Id };
            MediaCapture mediaCapture = new MediaCapture();
            await mediaCapture.InitializeAsync(mediaInitSettings);

            PreviewControl.Source = mediaCapture;
            await mediaCapture.StartPreviewAsync();

            await Task.Delay(2000);

            FaceDetector faceDetector = await FaceDetector.CreateAsync();

            // Get information about the preview
            var previewProperties = mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;

            /*var storageFile = await KnownFolders.CameraRoll.CreateFileAsync("filename1.jpg", CreationCollisionOption.GenerateUniqueName);

            await mediaCapture.CapturePhotoToStorageFileAsync(new ImageEncodingProperties {
            Height = 640,
            Width = 480,
            }, storageFile);*/

            // Create the video frame to request a SoftwareBitmap preview frame
             var videoFrame = new VideoFrame(BitmapPixelFormat.Nv12, (int)previewProperties.Width, (int)previewProperties.Height);

            // Capture the preview frame
            using (var currentFrame = await mediaCapture.GetPreviewFrameAsync(videoFrame))
            {
                // Collect the resulting frame
                SoftwareBitmap previewFrame = currentFrame.SoftwareBitmap;

                //PlayWithData(previewFrame);

                // Add a simple green filter effect to the SoftwareBitmap
                var result = await faceDetector.DetectFacesAsync(previewFrame);
                result.ToString();
            }
        }
        /*
        public SoftwareBitmap GetImage()
        {
            MediaFrameSourceGroup _holoLensMediaFrameSourceGroup = new MediaFrameSourceGroup(MediaFrameSourceGroupType.PhotoVideoCamera, new SpatialPerception(), null);

            SensorFrame latestFrame;
            latestFrame = _holoLensMediaFrameSourceGroup.GetLatestSensorFrame(SensorType.PhotoVideo);

            if (latestFrame == null || latestFrame.Timestamp == lastFrameTimestamp)
                return null;

            lastFrameTimestamp = latestFrame.Timestamp;

            webcamToWorldMatrix.m00 = latestFrame.FrameToOrigin.M11;
            webcamToWorldMatrix.m01 = latestFrame.FrameToOrigin.M21;
            webcamToWorldMatrix.m02 = latestFrame.FrameToOrigin.M31;

            webcamToWorldMatrix.m10 = latestFrame.FrameToOrigin.M12;
            webcamToWorldMatrix.m11 = latestFrame.FrameToOrigin.M22;
            webcamToWorldMatrix.m12 = latestFrame.FrameToOrigin.M32;

            webcamToWorldMatrix.m20 = -latestFrame.FrameToOrigin.M13;
            webcamToWorldMatrix.m21 = -latestFrame.FrameToOrigin.M23;
            webcamToWorldMatrix.m22 = -latestFrame.FrameToOrigin.M33;

            webcamToWorldMatrix.m03 = latestFrame.FrameToOrigin.Translation.X;
            webcamToWorldMatrix.m13 = latestFrame.FrameToOrigin.Translation.Y;
            webcamToWorldMatrix.m23 = -latestFrame.FrameToOrigin.Translation.Z;
            webcamToWorldMatrix.m33 = 1;


            if (imageInitialized == false)
            {
                height = latestFrame.SoftwareBitmap.PixelHeight;
                width = latestFrame.SoftwareBitmap.PixelWidth;

                projectionMatrix = new Matrix4x4();
                projectionMatrix.m00 = 2 * latestFrame.CameraIntrinsics.FocalLength.X / width;
                projectionMatrix.m11 = 2 * latestFrame.CameraIntrinsics.FocalLength.Y / height;
                projectionMatrix.m02 = -2 * (latestFrame.CameraIntrinsics.PrincipalPoint.X - width / 2) / width;
                projectionMatrix.m12 = 2 * (latestFrame.CameraIntrinsics.PrincipalPoint.Y - height / 2) / height;
                projectionMatrix.m22 = -1;
                projectionMatrix.m33 = -1;

                imageInitialized = true;
            }

            


            return latestFrame.SoftwareBitmap;
        }*/

        private async void PlayWithData(SoftwareBitmap softwareBitmap)
        {
            // get encoded jpeg bytes
            var data = await EncodedBytes(softwareBitmap, BitmapEncoder.PngEncoderId);

            File.WriteAllBytes("out.jpg", data);
            // todo: save the bytes to a DB, etc
        }

        private async Task<byte[]> EncodedBytes(SoftwareBitmap soft, Guid encoderId)
        {
            byte[] array = null;

            // First: Use an encoder to copy from SoftwareBitmap to an in-mem stream (FlushAsync)
            // Next:  Use ReadAsync on the in-mem stream to get byte[] array

            using (var ms = new InMemoryRandomAccessStream())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(encoderId, ms);
                encoder.SetSoftwareBitmap(soft);

                try
                {
                    await encoder.FlushAsync();
                }
                catch (Exception ex) { return new byte[0]; }

                array = new byte[ms.Size];
                await ms.ReadAsync(array.AsBuffer(), (uint)ms.Size, InputStreamOptions.None);
            }
            return array;
        }
    }
}
