using Accord.Imaging.Filters;
using Accord.Vision.Detection;
using Accord.Vision.Detection.Cascades;
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

        private readonly HaarObjectDetector detector;
        

        public FaceDetectionManager()
        {
            HaarCascade cascade = new FaceHaarCascade();
            detector = new HaarObjectDetector(cascade, 30);
        }

        public void Configuration()
        {
            //Detector.SearchMode = (ObjectDetectorSearchMode)cbMode.SelectedValue;
            //Detector.ScalingMode = (ObjectDetectorScalingMode)cbScaling.SelectedValue;
            detector.ScalingFactor = 1.5f;
            detector.UseParallelProcessing = true;
            detector.Suppression = 2;
        }

        public Bitmap DetectFace(Bitmap _bitmap)
        {
            Rectangle[] objects = detector.ProcessFrame(_bitmap);

            if (objects.Length > 0)
            {
                RectanglesMarker marker = new RectanglesMarker(objects, Color.Fuchsia);
                return marker.Apply(_bitmap);
            }

            return _bitmap;
        }
    }
}
