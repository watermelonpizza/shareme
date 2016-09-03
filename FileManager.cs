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
        public async static Task<string> WriteFile(string fileExtension, IFormFile file, string uploadFolder)
        {
            EnsureDirectory(uploadFolder);

            // Generate a file name, keep going if you get a collision with an existing file
            string fileName = string.Empty;
            do
            {
                fileName = $"{RandomGenerator.GetRandomString(7)}.{fileExtension}";
            } while (File.Exists(Path.Combine(DirectoryPath(uploadFolder), $"{fileName}.{fileExtension}")));

            try
            {
                string systemFilePath = Path.Combine(DirectoryPath(uploadFolder), fileName);

                using (var stream = File.Create(systemFilePath))
                {
                    await file.CopyToAsync(stream);
                }

                return fileName;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static string DirectoryPath(string uploadFolder) => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", uploadFolder);

        private static void EnsureDirectory(string uploadFolder)
        {
            Directory.CreateDirectory(DirectoryPath(uploadFolder));
        }
    }
}
