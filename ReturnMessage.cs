using Newtonsoft.Json;

namespace ShareMe
{
    public class ReturnMessage
    {
        public bool Ok { get; private set; }
        public string Error { get; private set; }
        public string Warning { get; private set; }
        public string Message { get; private set; }
        public string[] FileUrls { get; private set; }
        public string Token { get; private set; }

        public static string OkFileUploaded(string message, string[] fileUrls)
            => JsonConvert.SerializeObject(new ReturnMessage
                {
                    Ok = true,
                    Message = message,
                    FileUrls = fileUrls
                },
                Program.JsonSettings);

        public static string OkFileDeleted(string message)
            => JsonConvert.SerializeObject(new ReturnMessage
                {
                    Ok = true,
                    Message = message,
                },
                Program.JsonSettings);

        public static string OkTokenMessage(string message, string token)
            => JsonConvert.SerializeObject(new ReturnMessage
                {
                    Ok = true,
                    Message = message,
                    Token = token
                },
                Program.JsonSettings);

        public static string WarningMessage(string warning)
            => JsonConvert.SerializeObject(new ReturnMessage
                {
                    Ok = true,
                    Warning = warning
                },
                Program.JsonSettings);

        public static string ErrorMessage(string error)
            => JsonConvert.SerializeObject(new ReturnMessage
                {
                    Ok = false,
                    Error = error
                },
                Program.JsonSettings);
    }
}
