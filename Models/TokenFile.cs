using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareMe.Models
{
    public struct TokenFile
    {
        public List<Token> Tokens;

        public TokenFile(List<Token> tokens)
        {
            Tokens = tokens;
        }
    }

    public struct Token
    {
        public string Key;
        public string Comment;

        public Token(string key, string comment)
        {
            Key = key;
            Comment = comment;
        }
    }
}
