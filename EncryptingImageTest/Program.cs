using CryptingLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptingImageTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Bitmap bitmap = new Bitmap(@"C:\Users\phjacob\Desktop\test\Crypto.jpg");
            //byte[] EncBytes = CryptingLib.Algorithm.EncryptingAlgorithm.Encrypt(CryptingLib.Algorithm.EncryptingAlgorithm.ImageToByte(bitmap));
            //Image EncPicture = CryptingLib.Algorithm.EncryptingAlgorithm.ByteArrayToImage(EncBytes);

            Bitmap EncPicture = CryptingLib.Algorithm.EncryptingAlgorithm.TestCryptingImage(bitmap);
            


            //byte[] Bytes = RajindaelCrypt.GetBytes(bitmap);
            //byte[] EncBytes = RajindaelCrypt.Encrypt(Bytes, "zaodahdlabdnla");
            //Image EncPicture = RajindaelCrypt.GetImage(EncBytes);
            //EncPicture.GetThumbnailImage(bitmap.Width, bitmap.Height, null, System.IntPtr.Zero);

            EncPicture.Save(@"C:\Users\phjacob\Desktop\test\Crypted.bmp");


            //sbi = BitmapHelper.Bitmap2BitmapImage((Bitmap)EncPicture);
        }
    }
}
