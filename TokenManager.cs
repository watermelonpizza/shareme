using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ShareMe.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ShareMe
{
    internal static class TokenManager
    {
        private static RandomNumberGenerator rand = RandomNumberGenerator.Create();
        private static List<Token> tokens = new List<Token>();

        private static string adminToken;

        public static string AdminToken
        {
            get { return adminToken; }
        }

        public static IReadOnlyList<Token> Tokens
        {
            get { return tokens; }
        }

        static TokenManager()
        {
            string error;
            if (!LoadTokens(out error))
            {
                Console.WriteLine(error);
            }
        }

        public static bool HasToken(string token)
            => adminToken.Equals(token) || tokens.Any(t => t.Key.Equals(token));

        public static bool SaveTokens()
        {
            try
            {
                File.WriteAllText("tokens.json", JsonConvert.SerializeObject(new TokenFile(tokens)));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool LoadTokens(out string error)
        {
            try
            {
                adminToken = Environment.GetEnvironmentVariable("SHAREME_ADMIN_TOKEN");
                if (string.IsNullOrWhiteSpace(adminToken))
                {
                    error = "You must supply the environment variable SHAREME_ADMIN_TOKEN";
                    return false;
                }
            }
            catch (SecurityException e)
            {
                error = e.Message;
                return false;
            }

            try
            {
                if (File.Exists("tokens.json"))
                {
                    using (StreamReader sr = new StreamReader(File.OpenRead("tokens.json")))
                    {
                        var tokenfile = JsonConvert.DeserializeObject<TokenFile>(sr.ReadToEnd());
                        tokens = tokenfile.Tokens;

                        error = string.Empty;
                        return true;
                    }
                }
                else
                {
                    error = "tokens.json file either corrupted or missing";
                    return false;
                }
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }
        }

        public static Token CreateToken()
        {
            string key = RandomGenerator.GetRandomHex(32);
            Token newToken = new Token(key, $"autogen key {DateTime.Now.ToString("yyyyMMddhhmmssffffff")}");
            tokens.Add(newToken);

            SaveTokens();
            return newToken;
        }

        public static Token CreateToken(string key, string comment)
        {
            Token newToken = new Token(key, comment);
            tokens.Add(newToken);
            SaveTokens();
            return newToken;
        }

        public static bool DeleteToken(string tokenKey)
        {
            tokens.Remove(tokens.First(t => t.Key.Equals(tokenKey)));
            SaveTokens();

            return true;
        }
    }
}
