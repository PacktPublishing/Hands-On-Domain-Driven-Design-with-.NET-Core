using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Marketplace.Modules.Images
{
    public static class ImageStorage
    {
        public static async Task<string> UploadFile(string file)
        {
            var data = Regex.Match(
                file, @"data:image/(?<type>.+?),(?<data>.+)"
            );
            var base64Data = data.Groups["data"].Value;
            var type = data.Groups["type"].Value.Split(';')[0];

            var bytes = Convert.FromBase64String(base64Data);
            var fileName = $"{Path.GetTempFileName()}.{type}";

            using (var imageFile = new FileStream(fileName, FileMode.Create))
            {
                await imageFile.WriteAsync(bytes, 0, bytes.Length);
                await imageFile.FlushAsync();
            }

            return fileName;
        }

        public static Task<byte[]> GetFile(string file)
            => File.ReadAllBytesAsync(file);
    }
}