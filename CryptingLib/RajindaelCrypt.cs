using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptingLib
{
    public class RajindaelCrypt
    {
            public static byte[] GetBytes(Bitmap b)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    b.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    byte[] ret = ms.ToArray();
                    return ret;
                }
            }

            public static Image GetImage(byte[] PictureBytes)
            {
            //Image tempImage;
            //using (MemoryStream ms = new MemoryStream(PictureBytes, 0, PictureBytes.Length))
            //{
            //    Console.WriteLine("Longueur Image cryptée = " + PictureBytes.Length);
            //    //ms.Write(PictureBytes, 0, PictureBytes.Length);
            //    tempImage = Image.FromStream(ms, true);
            //    return tempImage;
            //}

            using (Image img = Image.FromStream(new MemoryStream(PictureBytes)))
            {
                return img;
            }

            //ImageConverter converter = new System.Drawing.ImageConverter();
            //Image img = (Image)converter.ConvertFrom(PictureBytes);

            //return img;
        }

            public static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
            {
                if (clearData == null || clearData.Length <= 0)
                    throw new ArgumentNullException("clearData");
                if (Key == null || Key.Length <= 0)
                    throw new ArgumentNullException("Key");
                if (IV == null || IV.Length <= 0)
                    throw new ArgumentNullException("IV");

                byte[] encrypted;

                using (Rijndael rijndaelAlg = Rijndael.Create())
                {
                    rijndaelAlg.Key = Key;
                    rijndaelAlg.IV = IV;
                    ICryptoTransform encryptor = rijndaelAlg.CreateEncryptor(rijndaelAlg.Key, rijndaelAlg.IV);

                    // Create the streams used for encryption.
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            Console.WriteLine("Before Encryption, bitmap lenght = " + clearData.Length);
                            csEncrypt.Write(clearData, 0, clearData.Length);
                            csEncrypt.FlushFinalBlock();
                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }
                return encrypted;
            }

            public static byte[] Encrypt(byte[] clearData, string Password)
            {
                byte[] salt = CreateRandomSalt(16);

                PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, salt);
                return Encrypt(clearData, pdb.GetBytes(32), pdb.GetBytes(16));
            }

            public static byte[] CreateRandomSalt(int length)
            {
                // Create a buffer
                byte[] randBytes;

                if (length >= 1)
                    randBytes = new byte[length];
                else
                    randBytes = new byte[1];

                // Create a new RNGCryptoServiceProvider.
                RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();

                // Fill the buffer with random bytes.
                rand.GetBytes(randBytes);

                // return the bytes.
                return randBytes;
            }

        }
}
