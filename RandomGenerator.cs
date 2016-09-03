using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ShareMe
{
    public static class RandomGenerator
    {
        private const string availableCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        private static RandomNumberGenerator rand = RandomNumberGenerator.Create();

        public static string GetRandomHex(int length)
        {
            if (length > 1 && length % 2 == 0)
            {
                byte[] randomBytes = new byte[length / 2];
                rand.GetBytes(randomBytes);

                return BitConverter.ToString(randomBytes).Replace("-", string.Empty).ToLower();
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(length), length, "Must be even and > 1");
            }
        }


        public static string GetRandomString(int length)
        {
            if (length > 0)
            {
                StringBuilder sb = new StringBuilder(length);

                while (sb.Length != length)
                {
                    byte[] oneByte = new byte[1];
                    rand.GetBytes(oneByte);

                    char character = (char)oneByte[0];
                    if (availableCharacters.Contains(character))
                    {
                        sb.Append(character);
                    }
                }

                return sb.ToString();
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(length), length, "Must be greater than 0");
            }
        }
    }
}
