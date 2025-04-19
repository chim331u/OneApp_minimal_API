using System.Security.Cryptography;
using System.Text;
using fc_minimalApi.Interfaces;

namespace fc_minimalApi.Services
{
    public class UtilityServices : IUtilityServices
    {
        public string TimeDiff(DateTime start, DateTime end)
        {
            TimeSpan _span = end - start;
            return string.Concat(((int)_span.TotalMilliseconds).ToString(), " ms");
        }
        public string EncryptString(string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            string key = "CryptoKey";

            using (Aes aes = Aes.Create())
            {
                try
                {
                    byte[] keyBytes;
                    using (Rfc2898DeriveBytes pbkdf = new Rfc2898DeriveBytes(key, Encoding.UTF8.GetBytes(key)))
                    {
                        // here 16 bytes for AES128
                        keyBytes = pbkdf.GetBytes(16);
                    }

                    aes.Key = keyBytes;
                    aes.IV = iv;
                    aes.Padding = PaddingMode.PKCS7;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream =
                               new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                            {
                                streamWriter.Write(plainText);
                            }

                            array = memoryStream.ToArray();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return e.Message;
                }
           
            }

            return Convert.ToBase64String(array);
        }
        
        public string DecryptString(string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            string key = "CryptoKey";

            try
            {
                byte[] keyBytes;
                using (Rfc2898DeriveBytes pbkdf = new Rfc2898DeriveBytes(key, Encoding.UTF8.GetBytes(key)))
                {
                    // here 16 bytes for AES128
                    keyBytes = pbkdf.GetBytes(16);
                }
                
                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.IV = iv;
                    aes.Padding = PaddingMode.PKCS7;

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return e.Message;
            }
           
        }
    }
}