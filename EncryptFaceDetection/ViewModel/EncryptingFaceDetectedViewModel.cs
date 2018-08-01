using EncryptFaceDetection.BLL;
using EncryptFaceDetection.Events;
using EncryptFaceDetection.Helpers;
using EncryptFaceDetection.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace EncryptFaceDetection.ViewModel
{
    public class EncryptingFaceDetectedViewModel : ViewModelBase
    {
        #region Internal datas
        private readonly VideoManager videoManager;
        private readonly FaceDetectionManager faceDetectionManager;
        #endregion
        public ObservableCollection<CameraModel> AllVideoDevices { get; set; }        // Allows to display all Devices

        private BitmapImage _displayStream;
        public BitmapImage DisplayStream     // Allows to display the camera stream
        {
            get { return this._displayStream; }
            set
            {
                if (this._displayStream != value)
                {
                    this._displayStream = value;
                    RaisePropertyChanged();
                }

            }
        }

        private CameraModel _currentCamera;
        public CameraModel CurrentCamera
        {
            get { return this._currentCamera; }
            set
            {
                if (this._currentCamera != value)
                {
                    this._currentCamera = value;
                    RaisePropertyChanged();
                }

            }
        }

        private bool _isLoadingCameraStream;
        public bool IsLoadingCameraStream
        {
            get { return this._isLoadingCameraStream; }
            set
            {
                if (this._isLoadingCameraStream != value)
                {
                    this._isLoadingCameraStream = value;
                    RaisePropertyChanged();
                }

            }
        }


        #region Constructor

        public EncryptingFaceDetectedViewModel()
        {
            this.videoManager = new VideoManager();
            this.faceDetectionManager = new FaceDetectionManager();

            this.AllVideoDevices = this.videoManager.GetAllVideoDevices(); // put No Camera as default when we start the video

            this.IsLoadingCameraStream = false; // is used to control the loading image

            MessengerInstance.Register<string>(this, "Event", (action) => EventSent(action));
        }

        #endregion

        #region Internal function

        private void EventSent(string action)
        {
            if (action.Equals("Close"))
                this.OnCloseEvent();
        }

        private void StartCamera(CameraModel aCamera)
        {
            if (this.CurrentCamera.CurrentDevice != null)
            {
                this.IsLoadingCameraStream = true;
                this.DisplayStream = null; // put the image null for the progress ring

                this.videoManager.New_Frame += Video_NewFrame;
                this.videoManager.StartCamera(aCamera);
            }
            else
            {
                this.DisplayStream = null;
            }
        }

        private void StopCamera()
        {
            this.videoManager.New_Frame -= Video_NewFrame; // Before StopCamera => Permet to not receive more frame and display one of them
            this.videoManager.StopCamera();
        }

        #endregion

        #region Commands

        private ICommand _onLoadVideoWindowCommand;
        public ICommand OnLoadVideoWindowCommand
        {
            get
            {
                return _onLoadVideoWindowCommand ?? (_onLoadVideoWindowCommand = new RelayCommand(() => OnLoadVideoWindow()));
            }
        }

        private ICommand _onCloseEventCommand;
        public ICommand OnCloseEventCommand
        {
            get
            {
                return _onCloseEventCommand ?? (_onCloseEventCommand = new RelayCommand(() => this.OnCloseEvent()));
            }
        }

        #region Command functions

        private void OnLoadVideoWindow()
        {
            foreach (CameraModel camera in this.AllVideoDevices)
            {
                // Allow to don't work with our fictive camera which is null
                if (!String.IsNullOrEmpty(camera.Name))
                {
                    this.CurrentCamera = camera;
                }
            }

            this.StartCamera(this.CurrentCamera);

            this.faceDetectionManager.InitialTrackingConfiguration();
        }

        private void OnCloseEvent()
        {
            if (this.videoManager.IsCameraActivated)
            {
                this.StopCamera();
            }
        }

        #endregion



        #endregion

        #region Events

        private void Video_NewFrame(object source, NewCameraFrameEventArgs e)
        {
            try
            {
                if (IsLoadingCameraStream)
                {
                    this.IsLoadingCameraStream = false;
                }

                BitmapImage bi;

                using (var bitmap = e.GetBitmap())
                {
                    Bitmap trackBitmap = this.faceDetectionManager.FaceTracking(bitmap);

                    List<Point> first = new List<Point>();
                    List<Point> last = new List<Point>();

                    Rectangle rect = this.faceDetectionManager.FaceTracked;
                    first.Add(new Point(rect.X, rect.Y));
                    last.Add(new Point(rect.X + rect.Width, rect.Y + rect.Height));


                    Bitmap encryptedImage = CryptingLib.Algorithm.EncryptingAlgorithm.EncryptImage(trackBitmap, first, last);

                    bi = BitmapHelper.Bitmap2BitmapImage(encryptedImage);
                    //bi = BitmapHelper.Bitmap2BitmapImage(this.faceDetectionManager.FaceTracking(bitmap));
                }

                bi.Freeze(); // avoid cross thread operations and prevents leaks

                this.DisplayStream = bi;
            }
            catch (AForge.Video.VideoException exc)
            {
                this.videoManager.StopCamera();
                this.DisplayStream = null;
            }
        }

        #endregion


    }
}
