using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Vision.Detection;
using Accord.Vision.Detection.Cascades;
using Accord.Vision.Tracking;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptFaceDetection.BLL
{

    // http://accord-framework.net/samples.html

    class FaceDetectionManager
    {
        private HaarObjectDetector detector;
        private Camshift tracker;
        private RectanglesMarker marker;

        private bool isDetecting;
        private bool isTracking;
        private bool isCrypting = true;

        public Rectangle FaceTracked;

        public FaceDetectionManager()
        {
            this.detector = new HaarObjectDetector(new FaceHaarCascade(),
                25, ObjectDetectorSearchMode.Single, 1.2f,
                ObjectDetectorScalingMode.GreaterToSmaller);

            this.tracker = new Camshift();

            this.InitialTrackingConfiguration();

            this.FaceTracked = new Rectangle();
        }

        /// <summary>
        /// Allows to work with HamburgerMenu because item are not disposed at the close event
        /// </summary>
        public void InitialTrackingConfiguration()
        {
            this.isDetecting = true;
            this.isTracking = false;
        }

        public Bitmap FaceTracking(Bitmap _bitmap)
        {
            if (!isDetecting & !isTracking)
                return _bitmap;

            //if (FaceTracked.Length > 0)
            //    this.FaceTracked = null;

            if (this.isDetecting)
                return this.Detecting(_bitmap);
            else
            {
                if (this.isTracking)
                    return this.Tracking(_bitmap);
                else if (marker != null) return marker.Apply(_bitmap);
                return _bitmap;
            }
        }

        #region Internal function

        private Bitmap Tracking(Bitmap _bitmap)
        {
            UnmanagedImage im = UnmanagedImage.FromManagedImage(_bitmap);

            // Track the object
            tracker.ProcessFrame(im);

            // Get the object position
            var obj = tracker.TrackingObject;
            this.FaceTracked = obj.Rectangle;

            marker = new RectanglesMarker(obj.Rectangle);

            if (marker != null)
                marker.ApplyInPlace(im);

            return im.ToManagedImage();
        }

        private Bitmap Detecting(Bitmap _bitmap)
        {
            UnmanagedImage im = UnmanagedImage.FromManagedImage(_bitmap);

            float xscale = im.Width / 160f;
            float yscale = im.Height / 120f;

            ResizeNearestNeighbor resize = new ResizeNearestNeighbor(160, 120);
            UnmanagedImage downsample = resize.Apply(im);


            Rectangle[] regions = detector.ProcessFrame(downsample);


            if (regions.Length > 0)
            {
                tracker.Reset();

                // Reduce the face size to avoid tracking background
                Rectangle window = new Rectangle(
                    (int)((regions[0].X + regions[0].Width / 2f) * xscale),
                    (int)((regions[0].Y + regions[0].Height / 2f) * yscale),
                    1, 1);

                window.Inflate(
                    (int)(0.2f * regions[0].Width * xscale),
                    (int)(0.4f * regions[0].Height * yscale));

                this.FaceTracked = window;

                // Initialize tracker
                tracker.SearchWindow = window;
                tracker.ProcessFrame(im);

                marker = new RectanglesMarker(window);
                marker.ApplyInPlace(im);

                // (Bitmap) Helpers.BitmapHelper.ByteArrayToImage(Helpers.RijndaelHelper.EncryptBytes(Helpers.BitmapHelper.ImageToByte(im.ToManagedImage()), "fzafa", "afzd"))

                this.isTracking = true;

                return im.ToManagedImage();
            }
            else
            {
                this.isDetecting = false;
                return _bitmap;
            }
        }
        #endregion
    }
}
