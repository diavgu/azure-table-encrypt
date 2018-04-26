using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Security
{
    public static class Cryptography
    {
        #region Settings

        private static int _iterations = 2;
        private static int _keySize = 256;
        private static string _hash = "SHA1";

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Encrypt(string value, string password, out string iv, out string salt)
        {
            return Encrypt<AesManaged>(value, password, out iv, out salt);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Encrypt(string value, string password, string iv, string salt)
        {
            return Encrypt<AesManaged>(value, password, iv, salt);
        }

        private static string Encrypt<T>(string value, string password, out string iv, out string salt) where T : SymmetricAlgorithm, new()
        {
            iv = KeyGenerator.GetUniqueKey(16);
            salt = KeyGenerator.GetUniqueKey(16);

            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            var vectorBytes = Encoding.ASCII.GetBytes(iv);
            var saltBytes = Encoding.ASCII.GetBytes(salt);
            var valueBytes = Encoding.UTF8.GetBytes(value);

            byte[] encrypted;
            using (T cipher = new T())
            {
                var _passwordBytes = new PasswordDeriveBytes(password, saltBytes, _hash, _iterations);
                var keyBytes = _passwordBytes.GetBytes(_keySize / 8);

                cipher.Mode = CipherMode.CBC;

                using (var encryptor = cipher.CreateEncryptor(keyBytes, vectorBytes))
                {
                    using (var to = new MemoryStream())
                    {
                        using (var writer = new CryptoStream(to, encryptor, CryptoStreamMode.Write))
                        {
                            writer.Write(valueBytes, 0, valueBytes.Length);
                            writer.FlushFinalBlock();
                            encrypted = to.ToArray();
                        }
                    }
                }
                cipher.Clear();
            }
            return Convert.ToBase64String(encrypted);
        }

        private static string Encrypt<T>(string value, string password, string iv, string salt) where T : SymmetricAlgorithm, new()
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            var vectorBytes = Encoding.ASCII.GetBytes(iv);
            var saltBytes = Encoding.ASCII.GetBytes(salt);
            var valueBytes = Encoding.UTF8.GetBytes(value);

            byte[] encrypted;
            using (T cipher = new T())
            {
                var _passwordBytes = new PasswordDeriveBytes(password, saltBytes, _hash, _iterations);
                var keyBytes = _passwordBytes.GetBytes(_keySize / 8);

                cipher.Mode = CipherMode.CBC;

                using (var encryptor = cipher.CreateEncryptor(keyBytes, vectorBytes))
                {
                    using (var to = new MemoryStream())
                    {
                        using (var writer = new CryptoStream(to, encryptor, CryptoStreamMode.Write))
                        {
                            writer.Write(valueBytes, 0, valueBytes.Length);
                            writer.FlushFinalBlock();
                            encrypted = to.ToArray();
                        }
                    }
                }
                cipher.Clear();
            }
            return Convert.ToBase64String(encrypted);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Decrypt(string value, string password, string _vector, string _salt)
        {
            return Decrypt<AesManaged>(value, password, _vector, _salt);
        }

        private static string Decrypt<T>(string value, string password, string _vector, string _salt) where T : SymmetricAlgorithm, new()
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            byte[] vectorBytes = Encoding.ASCII.GetBytes(_vector);
            byte[] saltBytes = Encoding.ASCII.GetBytes(_salt);
            byte[] valueBytes = Convert.FromBase64String(value);

            byte[] decrypted;
            int decryptedByteCount = 0;

            using (T cipher = new T())
            {
                PasswordDeriveBytes _passwordBytes = new PasswordDeriveBytes(password, saltBytes, _hash, _iterations);
                byte[] keyBytes = _passwordBytes.GetBytes(_keySize / 8);

                cipher.Mode = CipherMode.CBC;

                try
                {
                    using (ICryptoTransform decryptor = cipher.CreateDecryptor(keyBytes, vectorBytes))
                    {
                        using (MemoryStream from = new MemoryStream(valueBytes))
                        {
                            using (CryptoStream reader = new CryptoStream(from, decryptor, CryptoStreamMode.Read))
                            {
                                decrypted = new byte[valueBytes.Length];
                                decryptedByteCount = reader.Read(decrypted, 0, decrypted.Length);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return String.Empty;
                }

                cipher.Clear();
            }
            return Encoding.UTF8.GetString(decrypted, 0, decryptedByteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Index(string input, string key)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            var sb = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                sb.Append((input[i] & key[(i % key.Length)]).ToString());
            }
            return sb.ToString();
        }
    }

    public class KeyGenerator
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetUniqueKey(int maxSize)
        {
            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            var data = new byte[1];
            using (var crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            var result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }
    }
}