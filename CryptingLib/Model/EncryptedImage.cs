using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptingLib.Model
{
    class EncryptedImage
    {
        public byte[] Crypted { get; set; }
        public byte[] Image { get; set; }
        public byte[] Iv { get; set; }

        public List<Point> First;
        public List<Point> Lasts;

        public EncryptedImage(byte[] _crypted, byte[] _image, byte[] _iv, List<Point> _firsts, List<Point> _lasts)
        {
            this.Crypted = _crypted;
            this.Image = _image;
            this.Iv = _iv;

            this.First = _firsts;
            this.Lasts = _lasts;
        }
    }
}
