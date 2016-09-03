using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShareMe
{
    public static class FileManager
    {
        private const string StorageFolder = "i";
        
        public async static Task<string> WriteFile(string fileExtension, IFormFile file)
        {
            EnsureDirectory();

            string fileName = string.Empty;
            do
            {
                fileName = $"{RandomGenerator.GetRandomString(7)}.{fileExtension}";
            } while (File.Exists(Path.Combine(DirectoryPath, $"{fileName}.{fileExtension}")));

            try
            {
                string fullFilePath = Path.Combine(DirectoryPath, fileName);

                using (var stream = File.Create(fullFilePath))
                {
                    await file.CopyToAsync(stream);
                }

                return $"/{StorageFolder}/{fileName}";
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static string DirectoryPath => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", StorageFolder);

        private static void EnsureDirectory()
        {
            Directory.CreateDirectory(DirectoryPath);
        }
    }
}
