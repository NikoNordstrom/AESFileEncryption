using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;

namespace AesFileEncryption
{
    class AesCrypt
    {
        //private static MainForm mf = null;

        private const int keysize = 256;
        private const int iterations = 4096;

        public static void Encrypt(string path, string password, bool keepFile, BackgroundWorker bgw = null)
        {
            try
            {
                //UpdateLog("Tervehdys toisesta luokasta! :)"); // ei toimi :(
                //return;

                File.Move(path, path + ".encrypted");
                path = path + ".encrypted";

                if (keepFile == true && !File.Exists(path.Replace(".encrypted", "")))
                {
                    File.Copy(path, path.Replace(".encrypted", ""));
                }

                byte[] saltBytes = Generate256BitsOfRandom();
                byte[] ivBytes = Generate256BitsOfRandom();

                using (var key = new Rfc2898DeriveBytes(password, saltBytes, iterations))
                {
                    byte[] keyBytes = key.GetBytes(keysize / 8);

                    using (var symmetricAlg = new RijndaelManaged())
                    {
                        symmetricAlg.BlockSize = 256;
                        symmetricAlg.Mode = CipherMode.CBC;
                        symmetricAlg.Padding = PaddingMode.PKCS7;

                        using (var encryptor = symmetricAlg.CreateEncryptor(keyBytes, ivBytes))
                        {
                            FileStream readStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Write);
                            FileStream writeStream = new FileStream(path, FileMode.Open, FileAccess.Write);

                            CryptoStream encryptStream = new CryptoStream(writeStream, encryptor, CryptoStreamMode.Write);

                            long fileSize = new FileInfo(path).Length;
                            int progressInt = 0;
                            double progress = 0;

                            byte[] buffer = new byte[4096];
                            int read;

                            while ((read = readStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                encryptStream.Write(buffer, 0, read);

                                double value = ((read / (double)fileSize) * 100);

                                if (value == 100 && bgw != null)
                                {
                                    progressInt = 100;
                                    bgw.ReportProgress(progressInt);
                                }
                                else if (value < 100 && bgw != null)
                                {
                                    progress += value;
                                    progressInt = (int)progress;
                                    bgw.ReportProgress(progressInt);
                                }
                            }
                            
                            encryptStream.FlushFinalBlock();
                            encryptStream.Close();

                            readStream.Close();
                            writeStream.Close();

                            SHA256 sha2 = SHA256.Create();

                            byte[] keyHashBytes = sha2.ComputeHash(keyBytes);
                            byte[] hmacBytes = new byte[32];

                            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                            {
                                fileStream.Position = 0;
                                
                                byte[] fileHashBytes = sha2.ComputeHash(fileStream);
                                hmacBytes = GenerateHMACBytes(keyBytes, fileHashBytes);

                                fileStream.Close();
                            }

                            using (FileStream appendStream = new FileStream(path, FileMode.Append, FileAccess.Write))
                            {
                                appendStream.Write(saltBytes, 0, saltBytes.Length);
                                appendStream.Write(ivBytes, 0, ivBytes.Length);

                                appendStream.Write(keyHashBytes, 0, keyHashBytes.Length);
                                appendStream.Write(hmacBytes, 0, hmacBytes.Length);

                                appendStream.Close();
                            }

                            if (progressInt != 100 && bgw != null)
                            {
                                bgw.ReportProgress(100);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error encrypting a file!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Decrypt(string path, string password, bool keepFile, BackgroundWorker bgw = null)
        {
            try
            {
                byte[] saltBytes = new byte[keysize / 8];
                byte[] ivBytes = new byte[keysize / 8];

                using (FileStream readFileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    readFileStream.Position = readFileStream.Length - ((keysize / 8) * 2 + 64);
                    
                    readFileStream.Read(saltBytes, 0, saltBytes.Length);
                    readFileStream.Read(ivBytes, 0, ivBytes.Length);

                    readFileStream.Close();
                }

                using (var key = new Rfc2898DeriveBytes(password, saltBytes, iterations))
                {
                    byte[] keyBytes = key.GetBytes(keysize / 8);

                    SHA256 sha2 = SHA256.Create();

                    byte[] hashKeyBytes = sha2.ComputeHash(keyBytes);
                    byte[] hashKeyBytes2 = new byte[32];

                    using (FileStream readHashStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        readHashStream.Position = readHashStream.Length - 64;

                        readHashStream.Read(hashKeyBytes2, 0, hashKeyBytes2.Length);
                        readHashStream.Close();
                    }

                    if (!Enumerable.SequenceEqual(hashKeyBytes, hashKeyBytes2))
                    {
                        MessageBox.Show("File is corrupted or password is incorrect!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    byte[] hmacBytes = new byte[32];
                    byte[] hmacBytes2 = new byte[32];

                    using (FileStream hmacStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
                    {
                        hmacStream.Position = hmacStream.Length - 32;
                        hmacStream.Read(hmacBytes2, 0, hmacBytes2.Length);

                        hmacStream.SetLength(hmacStream.Length - ((keysize / 8) * 2 + 64));
                        hmacStream.Position = 0;

                        byte[] hashBytes = sha2.ComputeHash(hmacStream);
                        hmacBytes = GenerateHMACBytes(keyBytes, hashBytes);

                        hmacStream.Close();
                    }

                    if (!Enumerable.SequenceEqual(hmacBytes, hmacBytes2))
                    {
                        using (FileStream appendStream = new FileStream(path, FileMode.Append, FileAccess.Write))
                        {
                            appendStream.Write(hashKeyBytes2, 0, hashKeyBytes2.Length);
                            appendStream.Write(hmacBytes2, 0, hmacBytes2.Length);

                            appendStream.Close();
                        }

                        MessageBox.Show("File is corrupted or it has been altered!", "HMAC does not match!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    using (var symmetricAlg = new RijndaelManaged())
                    {
                        symmetricAlg.BlockSize = 256;
                        symmetricAlg.Mode = CipherMode.CBC;
                        symmetricAlg.Padding = PaddingMode.PKCS7;

                        using (var decryptor = symmetricAlg.CreateDecryptor(keyBytes, ivBytes))
                        {
                            FileStream inputStream = new FileStream(path, FileMode.Open, FileAccess.Read);

                            CryptoStream decryptStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read);

                            FileStream outputStream = new FileStream(path.Replace(".encrypted", ""), FileMode.Create);

                            long fileSize = new FileInfo(path).Length;
                            int progressInt = 0;
                            double progress = 0;

                            byte[] buffer = new byte[4096];
                            int read;

                            while ((read = decryptStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                outputStream.Write(buffer, 0, read);

                                double value = ((read / (double)fileSize) * 100);

                                if (value == 100 && bgw != null)
                                {
                                    progressInt = 100;
                                    bgw.ReportProgress(progressInt);
                                }
                                else if (value < 100 && bgw != null)
                                {
                                    progress += value;
                                    progressInt = (int)progress;
                                    bgw.ReportProgress(progressInt);
                                }
                            }

                            outputStream.Flush();
                            outputStream.Close();

                            decryptStream.Close();
                            inputStream.Close();

                            using (FileStream appendStream = new FileStream(path, FileMode.Append, FileAccess.Write))
                            {
                                appendStream.Write(saltBytes, 0, saltBytes.Length);
                                appendStream.Write(ivBytes, 0, ivBytes.Length);

                                appendStream.Write(hashKeyBytes2, 0, hashKeyBytes2.Length);
                                appendStream.Write(hmacBytes2, 0, hmacBytes2.Length);

                                appendStream.Close();
                            }

                            if (keepFile == false && File.Exists(path))
                            {
                                File.Delete(path);
                            }

                            if (bgw != null)
                            {
                                bgw.ReportProgress(100);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error decrypting a file!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static byte[] GenerateHMACBytes(byte[] key, byte[] message)
        {
            HMACSHA256 hmacsha2 = new HMACSHA256(key);
            return hmacsha2.ComputeHash(message);
        }

        private static byte[] Generate256BitsOfRandom()
        {
            byte[] randomBytes = new byte[32];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        //private static void UpdateLog(string text)
        //{
        //    if (mf == null)
        //    {
        //        mf = new MainForm();
        //    }
        //    mf.LogAppend(text);
        //}
    }
}
