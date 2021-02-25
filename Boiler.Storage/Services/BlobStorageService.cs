using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Boiler.Storage.Services
{
    class BlobStorageService
    {
        private readonly BlobServiceClient m_BlobServiceClient;
        private BlobContainerClient m_BlobContainerClient;

        public BlobStorageService()
        {
            // TODO(selim): Blob connection string should come from user secrets
            m_BlobServiceClient = new BlobServiceClient("BLOB_CONNECTION_STRING_HERE");
        }

        public async Task DeleteAsync(string fileName, string containerName)
        {
            m_BlobContainerClient = m_BlobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = m_BlobContainerClient.GetBlobClient(fileName);
            await blobClient.DeleteAsync();
        }

        public async Task<Stream> DownloadAsync(string fileName, string containerName)
        {
            m_BlobContainerClient = m_BlobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = m_BlobContainerClient.GetBlobClient(fileName);
            var response = await blobClient.DownloadAsync();
            return response.Value.Content;
        }

        public List<string> GetNames(string containerNames)
        {
            var blobNames = new List<string>();
            m_BlobContainerClient = m_BlobServiceClient.GetBlobContainerClient(containerNames);
            return m_BlobContainerClient.GetBlobs().Select(b => b.Name).ToList();
        }

        public async Task<string> UploadAsync(Stream fileStream, string name, string containerName)
        {
            m_BlobContainerClient = m_BlobServiceClient.GetBlobContainerClient(containerName);
            await m_BlobContainerClient.CreateIfNotExistsAsync();
            await m_BlobContainerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);
            var blobClient = m_BlobContainerClient.GetBlobClient(name);
            await blobClient.UploadAsync(fileStream);
            return blobClient.Uri.AbsoluteUri;
        }

        public Task<string> UploadAsync(IFormFile file, string name, string containerName)
        {
            return UploadAsync(file.OpenReadStream(), name, containerName);
        }

        public Task<string> UploadAsync(byte[] file, string name, string containerName) 
        {
            using var stream = new MemoryStream(file);
            return UploadAsync(stream, name, containerName);
        }
    }
}
