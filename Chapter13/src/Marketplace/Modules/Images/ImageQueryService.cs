using System;
using System.Threading.Tasks;

namespace Marketplace.Modules.Images
{
    public class ImageQueryService
    {
        readonly Func<string, Task<byte[]>> _getFile;

        public ImageQueryService(Func<string, Task<byte[]>> getFile)
            => _getFile = getFile;

        public Task<byte[]> GetFile(string fileName) => _getFile(fileName);
    }
}