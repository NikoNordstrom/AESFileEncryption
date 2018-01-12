using System;
using System.Linq;
using System.Security.Cryptography;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;

namespace AesFileEncryption
{
    class AesCrypt
    {
        private const int keysize = 256;
        private const int iterations = 10000;

        public static void Encrypt(string path, string password, bool keepFile, BackgroundWorker bgw = null)
        {
            try
            {
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

                            // readStream fills the buffer variable with bytes from the file.
                            // encryptStream encrypts the buffer bytes and then writes them to the file.
                            while ((read = readStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                encryptStream.Write(buffer, 0, read);

                                // This code is just calculating the progress and then reporting that back to the backgroundworker.
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

                            // This part will generate HMAC bytes.
                            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                            {
                                fileStream.Position = 0;
                                
                                byte[] fileHashBytes = sha2.ComputeHash(fileStream);
                                hmacBytes = GenerateHMACBytes(keyBytes, fileHashBytes);

                                fileStream.Close();
                            }

                            // This stream will append saltBytes, ivBytes, keyHashBytes and hmacBytes to end of the file.
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

                // readFileStream will read saltBytes and ivBytes from end of the file.
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

                    // readHashStream will read hashed key bytes from end of the file.
                    using (FileStream readHashStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        readHashStream.Position = readHashStream.Length - 64;

                        readHashStream.Read(hashKeyBytes2, 0, hashKeyBytes2.Length);
                        readHashStream.Close();
                    }

                    // Compare both hashed keys.
                    if (!Enumerable.SequenceEqual(hashKeyBytes, hashKeyBytes2))
                    {
                        MessageBox.Show("File is corrupted or password is incorrect!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    
                    byte[] hmacBytes = new byte[32];
                    byte[] hmacBytes2 = new byte[32];

                    using (FileStream hmacStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
                    {
                        // hmacStream will read HMAC bytes from end of the file.
                        hmacStream.Position = hmacStream.Length - 32;
                        hmacStream.Read(hmacBytes2, 0, hmacBytes2.Length);

                        hmacStream.SetLength(hmacStream.Length - ((keysize / 8) * 2 + 64));
                        hmacStream.Position = 0;

                        byte[] hashBytes = sha2.ComputeHash(hmacStream);
                        hmacBytes = GenerateHMACBytes(keyBytes, hashBytes);

                        hmacStream.Close();
                    }

                    // Compare both HMAC byte arrays.
                    if (!Enumerable.SequenceEqual(hmacBytes, hmacBytes2))
                    {
                        // Append hashKeyBytes2 and hmacBytes2 back to the file.
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

                            // readStream fills the buffer variable with bytes from the file.
                            // decryptStream decrypts the buffer bytes and then writes them to the file.
                            while ((read = decryptStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                outputStream.Write(buffer, 0, read);

                                // This code is just calculating the progress and then reporting that back to the backgroundworker.
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

                            // Append saltBytes, ivBytes, hashKeyBytes2 and hmacBytes2 back to the file.
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
    }
}
