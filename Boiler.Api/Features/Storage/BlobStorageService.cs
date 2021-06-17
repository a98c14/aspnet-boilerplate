using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

namespace Boiler.Core.Storage
{
    class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient m_BlobServiceClient;
        private readonly BlobStorageSettings m_BlobSettings;
        private BlobContainerClient m_BlobContainerClient;
        private FileExtensionContentTypeProvider m_fileExtensionProvider;

        public BlobStorageService(IOptions<BlobStorageSettings> settings)
        {
            m_BlobSettings = settings.Value;
            m_BlobServiceClient = new BlobServiceClient(m_BlobSettings.ConnectionString);
            m_fileExtensionProvider = new FileExtensionContentTypeProvider();
        }

        public async Task<bool> DeleteAsync(string fileName)
        {
            try
            {
                m_BlobContainerClient = m_BlobServiceClient.GetBlobContainerClient(m_BlobSettings.ContainerName);
                var blobClient = m_BlobContainerClient.GetBlobClient(fileName);
                await blobClient.DeleteAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Stream> DownloadAsync(string fileName)
        {
            m_BlobContainerClient = m_BlobServiceClient.GetBlobContainerClient(m_BlobSettings.ContainerName);
            var blobClient = m_BlobContainerClient.GetBlobClient(fileName);
            var response = await blobClient.DownloadAsync();
            return response.Value.Content;
        }

        public List<string> GetNames()
        {
            var blobNames = new List<string>();
            m_BlobContainerClient = m_BlobServiceClient.GetBlobContainerClient(m_BlobSettings.ContainerName);
            return m_BlobContainerClient.GetBlobs().Select(b => b.Name).ToList();
        }

        public List<string> GetNames(string containerNames)
        {
            throw new NotImplementedException();
        }

        public async Task<string> UploadAsync(Stream fileStream, string name)
        {
            m_BlobContainerClient = m_BlobServiceClient.GetBlobContainerClient(m_BlobSettings.ContainerName);
            await m_BlobContainerClient.CreateIfNotExistsAsync();
            await m_BlobContainerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);
            var blobClient = m_BlobContainerClient.GetBlobClient(name);
            var blobOptions = new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = GetContentTypeFromFileName(name) } };
            await blobClient.UploadAsync(fileStream, blobOptions);
            return $"{m_BlobContainerClient.Uri}/{blobClient.Name}";
        }

        public async Task<string> UploadAsync(IFormFile file, string name)
        {
            return await UploadAsync(file.OpenReadStream(), name);
        }

        public async Task<string> UploadAsync(byte[] file, string name)
        {
            using var stream = new MemoryStream(file);
            return await UploadAsync(stream, name);
        }

        public async Task<string> UploadAsync(string base64, string name)
        {
            return await UploadAsync(Convert.FromBase64String(base64), name);
        }
        private string GetContentTypeFromFileName(string fileName)
        {
            string contentType;
            if (!m_fileExtensionProvider.TryGetContentType(fileName, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}
