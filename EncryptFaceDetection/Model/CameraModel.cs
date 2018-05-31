using AForge.Video.DirectShow;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptFaceDetection.Model
{
    public class CameraModel : ObservableObject
    {
        public VideoCaptureDevice VideoSource { get; set; }

        public string Name { get; private set; }

        public VideoCapabilities VideoCapabilities { get; set; }

        private FilterInfo _currentDevice;
        public FilterInfo CurrentDevice
        {
            get { return this._currentDevice; }
            set
            {
                if (this._currentDevice != value)
                {
                    this._currentDevice = value;
                    RaisePropertyChanged();
                }

            }
        }

        public CameraModel(FilterInfo fi)
        {
            this.CurrentDevice = fi;
            this.Name = CurrentDevice.Name;
            this.VideoSource = new VideoCaptureDevice(fi.MonikerString);
        }

    }
}
