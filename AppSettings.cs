using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShareMe
{
    public class AppSettings
    {
        public string HostUrl { get; set; }
        public string UploadFolder { get; set; }
        public string FileRequestPath { get; set; }
        public string AdminKey { get; set; }
        public string[] ExtensionBlacklist { get; set; }
        public long MaxUploadSizeInBytes { get; set; }

        public string CloudFlareZone { get; set; }
        public string CloudFlareEmail { get; set; }
        public string CloudFlareKey { get; set; }

        public string PhysicalUploadPath
        {
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), UploadFolder);
            }
        }
    }
}
