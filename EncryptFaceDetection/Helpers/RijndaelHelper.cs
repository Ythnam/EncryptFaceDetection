using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EncryptFaceDetection.Helpers
{

    // Thx to https://stackoverflow.com/questions/4438691/simple-encryption-decryption-method-for-encrypting-an-image-file

    // Thx https://www.c-sharpcorner.com/forums/rijndael-image-encryption-problem
    public static class RijndaelHelper
    {
        public static byte[] GetBytes(Bitmap b)
        {
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    b.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            //    byte[] ret = ms.ToArray();
            //    return ret;
            //}
            return BitmapHelper.ImageToByte(b);
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

            return BitmapHelper.ByteArrayToImage(PictureBytes);
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

        //// Example usage: EncryptBytes(someFileBytes, "SensitivePhrase", "SodiumChloride");
        //public static byte[] EncryptBytes(byte[] inputBytes, string passPhrase, string saltValue)
        //{
        //    RijndaelManaged RijndaelCipher = new RijndaelManaged();

        //    RijndaelCipher.Mode = CipherMode.CBC;
        //    byte[] salt = Encoding.ASCII.GetBytes(saltValue);
        //    PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, salt, "SHA1", 2);

        //    ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(password.GetBytes(32), password.GetBytes(16));

        //    MemoryStream memoryStream = new MemoryStream();
        //    CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);
        //    cryptoStream.Write(inputBytes, 0, inputBytes.Length);
        //    cryptoStream.FlushFinalBlock();
        //    byte[] CipherBytes = memoryStream.ToArray();

        //    memoryStream.Close();
        //    cryptoStream.Close();

        //    return CipherBytes;
        //}

        //// Example usage: DecryptBytes(encryptedBytes, "SensitivePhrase", "SodiumChloride");
        //public static byte[] DecryptBytes(byte[] encryptedBytes, string passPhrase, string saltValue)
        //{
        //    RijndaelManaged RijndaelCipher = new RijndaelManaged();

        //    RijndaelCipher.Mode = CipherMode.CBC;
        //    byte[] salt = Encoding.ASCII.GetBytes(saltValue);
        //    PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, salt, "SHA1", 2);

        //    ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(password.GetBytes(32), password.GetBytes(16));

        //    MemoryStream memoryStream = new MemoryStream(encryptedBytes);
        //    CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);
        //    byte[] plainBytes = new byte[encryptedBytes.Length];

        //    int DecryptedCount = cryptoStream.Read(plainBytes, 0, plainBytes.Length);

        //    memoryStream.Close();
        //    cryptoStream.Close();

        //    return plainBytes;
        //}

    }
}
