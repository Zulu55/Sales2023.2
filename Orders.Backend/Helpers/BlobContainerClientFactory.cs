using Azure.Storage.Blobs;
using System.Diagnostics.CodeAnalysis;

namespace Orders.Backend.Helpers
{
    [ExcludeFromCodeCoverage(Justification = "It is a wrapper used to test other classes. There is no way to prove it.")]
    public class BlobContainerClientFactory : IBlobContainerClientFactory
    {
        public IBlobContainerClient CreateBlobContainerClient(string connectionString, string containerName) => new BlobContainerClientWrapper(connectionString, containerName);
    }
}