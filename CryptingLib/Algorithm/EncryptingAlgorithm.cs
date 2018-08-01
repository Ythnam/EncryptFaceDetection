using CryptingLib.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptingLib.Algorithm
{
    //https://stackoverflow.com/questions/21757983/get-the-distorted-encrypted-image-from-encrypted-data-bytes-of-an-image
    public static class EncryptingAlgorithm
    {

        public static Bitmap EncryptImage(Bitmap _bitmap , List<Point> _firsts, List<Point> _lasts)
        {
            Random rand = new Random();

            // https://stackoverflow.com/questions/1955766/iterate-two-lists-or-arrays-with-one-foreach-statement-in-c-sharp/1955780
            var firstsAndLasts = _firsts.Zip(_lasts, (f, l) => new { First = f, Last = l });

            foreach(var fl in firstsAndLasts)
            {
                Point p1 = fl.First;
                Point p2 = fl.Last;

                for (int x = p1.X; x < p2.X; x++)
                {
                    for (int y = p1.Y; y < p2.Y; y++)
                        if(x <_bitmap.Width && y < _bitmap.Height)_bitmap.SetPixel(x, y, Color.FromArgb(rand.Next(1, 255), rand.Next(1, 255), rand.Next(1, 255)));
                }
            }

            
            return _bitmap;
        }

        #region Bitmap Operation
        // Thx https://stackoverflow.com/questions/7350679/convert-a-bitmap-into-a-byte-array
        public static byte[] ImageToByte(Image img)
        {
            MemoryStream ms = new MemoryStream();
            //BitmapData date = 
            //img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            return ms.ToArray();

            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        //https://stackoverflow.com/questions/9173904/byte-array-to-image-conversion
        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            Image returnImage;

            using (MemoryStream ms = new MemoryStream(byteArrayIn))
            {
                returnImage = Image.FromStream(ms);
            }
            
            return returnImage;
        }
        #endregion

        // Thx https://simpledotnetsolutions.wordpress.com/2012/03/22/using-rijndaelmanaged-to-encryptdecrypt/ for the function which I modified for my case
        public static byte[] Encrypt(byte[] _plainText)
        {
            //In live, get the persistant passphrase from other isolated source
            //This example has hardcoded passphrase just for demo purpose
            StringBuilder sb = new StringBuilder();
            sb.Append("My_Encryption_Key");

            //Generate the Salt, with any custom logic and
            //using the above string
            StringBuilder _sbSalt = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                _sbSalt.Append("," + sb.Length.ToString());
            }
            byte[] Salt = Encoding.ASCII.GetBytes(_sbSalt.ToString());

            //Key generation:- default iterations is 1000 
            //and recomended is 10000
            Rfc2898DeriveBytes pwdGen = new Rfc2898DeriveBytes(sb.ToString(), Salt, 10000);

            //The default key size for RijndaelManaged is 256 bits, 
            //while the default blocksize is 128 bits.
            RijndaelManaged _RijndaelManaged = new RijndaelManaged();
            _RijndaelManaged.BlockSize = 256; //Increased it to 256 bits- max and more secure

            byte[] key = pwdGen.GetBytes(_RijndaelManaged.KeySize / 8);   //This will generate a 256 bits key
            byte[] iv = pwdGen.GetBytes(_RijndaelManaged.BlockSize / 8);  //This will generate a 256 bits IV

            //On a given instance of Rfc2898DeriveBytes class,
            //GetBytes() will always return unique byte array.
            _RijndaelManaged.Key = key;
            _RijndaelManaged.IV = iv;

            //Now encrypt
            byte[] cipherText2 = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, _RijndaelManaged.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(_plainText, 0, _plainText.Length);
                }
                cipherText2 = ms.ToArray();
            }
            return cipherText2;
        }

        public static Bitmap TestCryptingImage(Bitmap _bitmap /*, List<Point> _firsts, List<Point> _lasts*/)
        {
            Random rand = new Random();

            Point p1 = new Point(40, 100);
            Point p2 = new Point(70, 240);

            for (int x = p1.X; x < p2.X; x++)
            {
                for (int y = p1.Y; y < p2.Y; y++)
                    _bitmap.SetPixel(x, y, Color.FromArgb(rand.Next(1,255), rand.Next(1, 255), rand.Next(1, 255)));
            }
            return _bitmap;
        }
    }
}
