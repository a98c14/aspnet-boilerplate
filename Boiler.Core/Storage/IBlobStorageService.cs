using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Boiler.Core.Storage
{
   public interface IBlobStorageService
   {
        List<string> GetNames(string containerNames);
        Task<string> UploadAsync(Stream fileStream, string name);
        Task<string> UploadAsync(IFormFile file, string name);
        Task<string> UploadAsync(byte[] file, string name);
        Task<string> UploadAsync(string base64, string name);
        Task<Stream> DownloadAsync(string blobUrl);
        Task<bool>   DeleteAsync(string blobUrl);
   }
}