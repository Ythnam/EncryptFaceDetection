using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptFaceDetection.Events
{
    public delegate void NewCameraFrameEvent(object source, NewCameraFrameEventArgs e);

    public class NewCameraFrameEventArgs : EventArgs
    {
        private readonly Bitmap EventBitmap;
        public NewCameraFrameEventArgs(Bitmap _bitmap)
        {
            EventBitmap = _bitmap;
        }
        public Bitmap GetBitmap()
        {
            return EventBitmap;
        }
    }
}
