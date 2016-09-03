using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareMe
{
    public class ReturnMessage
    {
        private static JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public bool Ok { get; private set; }
        public string Error { get; private set; }
        public string Warning { get; private set; }
        public string Message { get; private set; }
        public string FileUrl { get; private set; }
        public string Token { get; private set; }

        public static string OkFileUploaded(string message, string fileUrl)
            => JsonConvert.SerializeObject(new ReturnMessage
                {
                    Ok = true,
                    Message = message,
                    FileUrl = fileUrl
                }, 
                jsonSettings);

        public static string OkTokenMessage(string message, string token)
            => JsonConvert.SerializeObject(new ReturnMessage
                {
                    Ok = true,
                    Message = message,
                    Token = token
                },
                jsonSettings);

        public static string WarningMessage(string warning)
            => JsonConvert.SerializeObject(new ReturnMessage
                {
                    Ok = true,
                    Warning = warning
                },
                jsonSettings);

        public static string ErrorMessage(string error)
            => JsonConvert.SerializeObject(new ReturnMessage
                {
                    Ok = false,
                    Error = error
                },
                jsonSettings);
    }
}
