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

        private List<CameraResolutionModel> _resolution;
        public List<CameraResolutionModel> Resolutions
        {
            get { return this._resolution; }
            set
            {
                if (this._resolution != value)
                {
                    this._resolution = value;
                    RaisePropertyChanged();
                }
            }
        }

        private CameraResolutionModel _currentResolution;
        public CameraResolutionModel CurrentResolution
        {
            get { return this._currentResolution; }
            set
            {
                if (this._currentResolution != value)
                {
                    this._currentResolution = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _zoomMultiplicator;
        public double ZoomMultiplicator
        {
            get { return this._zoomMultiplicator; }
            set
            {
                if (this._zoomMultiplicator != value)
                {
                    this._zoomMultiplicator = value;
                    RaisePropertyChanged();
                }

            }
        }

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

        public CameraModel()
        {
            this.CurrentDevice = null;
            this.ZoomMultiplicator = 1;
            this.Name = "No Camera";
            this.Resolutions = new List<CameraResolutionModel>();
        }

        public CameraModel(FilterInfo fi)
        {
            this.CurrentDevice = fi;
            this.ZoomMultiplicator = 1;
            this.Name = CurrentDevice.Name;
            this.Resolutions = new List<CameraResolutionModel>();
            this.VideoSource = new VideoCaptureDevice(fi.MonikerString);
        }

    }
}
