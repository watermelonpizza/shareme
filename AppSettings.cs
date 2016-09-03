using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareMe
{
    public class AppSettings
    {
        public string HostUrl { get; set; }
        public string UploadFolder { get; set; }
        public string ImageHostDirectory { get; set; }
        public string AdminKey { get; set; }
        public string[] ExtensionBlacklist { get; set; }
    }
}
