using AForge.Video;
using AForge.Video.DirectShow;
using EncryptFaceDetection.Events;
using EncryptFaceDetection.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptFaceDetection.BLL
{
    class VideoManager
    {

        #region Internal datas

        private CameraModel currentCamera;

        #endregion

        #region Properties

        public event NewCameraFrameEvent New_Frame; // Allows to send the current frame to the ViewModel

        public bool IsCameraActivated { get; set; }  // Allows to know if camera is Running but needn't camera instanciation

        #endregion

        #region External Function

        /// <summary>
        /// This method allows to get all Video Device on the computer
        /// </summary>
        /// <returns> a list of all those devices </returns>
        public ObservableCollection<CameraModel> GetAllVideoDevices()
        {
            ObservableCollection<CameraModel> allCamera = new ObservableCollection<CameraModel>();

            foreach (FilterInfo filterInfo in new FilterInfoCollection(FilterCategory.VideoInputDevice))
            {
                allCamera.Add(new CameraModel(filterInfo));
            }

            return allCamera;
        }

        /// <summary>
        /// Allow to Start a camera
        /// </summary>
        /// <param name="_camera"> Camera we want to start </param>
        public void StartCamera(CameraModel _camera)
        {
            if (_camera.CurrentDevice != null)
            {
                _camera.VideoSource = new VideoCaptureDevice(_camera.CurrentDevice.MonikerString);
                _camera.VideoSource.NewFrame += video_NewFrame;
                _camera.VideoSource.Start();

                this.currentCamera = _camera; // Save current camera

                this.IsCameraActivated = true;
            }
        }

        /// <summary>
        /// Allows to Stop the current camera
        /// </summary>
        public void StopCamera()
        {
            if (this.currentCamera.VideoSource != null && this.currentCamera.VideoSource.IsRunning)
            {
                this.currentCamera.VideoSource.SignalToStop();
                this.currentCamera.VideoSource.NewFrame -= video_NewFrame;

                System.Threading.SpinWait.SpinUntil(() => this.currentCamera.VideoSource.IsRunning == false);

                this.IsCameraActivated = false;
            }
        }

        /// <summary>
        /// Allows to open Camera properties (it's a Aforge function that allows this)
        /// </summary>
        public void ShowCameraProperties(CameraModel camera)
        {
            if (this.IsCameraActivated)
                camera.VideoSource.DisplayPropertyPage(IntPtr.Zero);
        }

        #endregion

        #region Event

        public void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    if (this.New_Frame != null)
                        New_Frame(this, new NewCameraFrameEventArgs(bitmap));
                }
            }
            catch (VideoException exc)
            {
                StopCamera();
                Console.WriteLine(exc);
            }
        }

        #endregion
    }
}
