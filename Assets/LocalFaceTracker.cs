#if WINDOWS_UWP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Media.FaceAnalysis;
using Windows.Graphics.Imaging;

using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

using UnityEngine;

namespace FaceProcessing
{
    public class LocalFaceTracker
    {
        private FaceDetector faceDetector;
        private FaceAligner faceAligner;

        private byte[] imageData;
        private float confidenceThreshold;
        private int nIters;

        public LocalFaceTracker(int nIters, float confidenceThreshold)
        {
            this.confidenceThreshold = confidenceThreshold;
            this.nIters = nIters;
            faceDetector = FaceDetector.CreateAsync().AsTask().Result;
            faceAligner = new FaceAligner(Application.dataPath + "/StreamingAssets/LocalFaceTracker/", nIters);
        }


        public async Task<bool> GetLandmarks(SoftwareBitmap image)
        {
            var boundingBoxes = await DetectFace(image);
               Debug.Log(">>>>" + boundingBoxes.Count);

            if (boundingBoxes.Count == 0)
                return false;

            return true;
        }

        private async Task<List<BitmapBounds>> DetectFace(SoftwareBitmap image)
        {
            if (imageData == null || imageData.Length != image.PixelHeight * image.PixelWidth)
            {
                imageData = new byte[image.PixelHeight * image.PixelWidth];
            }
            unsafe
            {
                fixed (byte* grayPointer = imageData)
                {
                    FaceProcessing.ImageProcessing.ColorToGrayscale(image, (Int32)(grayPointer));
                }
            }

            SoftwareBitmap grayImage = SoftwareBitmap.CreateCopyFromBuffer(imageData.AsBuffer(), BitmapPixelFormat.Gray8, image.PixelWidth, image.PixelHeight);         
            var faces = await faceDetector.DetectFacesAsync(grayImage);

            List<BitmapBounds> boundingBoxes = new List<BitmapBounds>();
            for (int i = 0; i < faces.Count; i++)
            {
                boundingBoxes.Add(faces[i].FaceBox);
            }

            return boundingBoxes;
        }
    }
}
#else

namespace FaceProcessing
{
    //Empty classes provided so that the project compiles in Editor
    class Task<TResult>
    {
        public Task()
        {
        }

        public TResult Result
        {
            get { return val; }
        }

        private TResult val;
    }

    class LocalFaceTracker
    {
        public bool ResetModelFitter
        {
            get
            {
                return true;
            }
        }

        public LocalFaceTracker(int nIters, float confidenceThreshold)
        {
        }

        public Task<float[]> GetLandmarks(byte[] imageData, int height, int width)
        {
            return new Task<float[]>();
        }
    }
}
#endif