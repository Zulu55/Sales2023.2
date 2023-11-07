using Azure.Core.Extensions;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Extensions.Azure;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage(Justification = "It is a wrapper used to test other classes. There is no way to prove it.")]
internal static class AzureClientFactoryBuilderExtensions
{
    public static IAzureClientBuilder<BlobServiceClient, BlobClientOptions> AddBlobServiceClient(this AzureClientFactoryBuilder builder, string serviceUriOrConnectionString, bool preferMsi)
    {
        if (preferMsi && Uri.TryCreate(serviceUriOrConnectionString, UriKind.Absolute, out Uri? serviceUri))
        {
            return builder.AddBlobServiceClient(serviceUri);
        }
        else
        {
            return builder.AddBlobServiceClient(serviceUriOrConnectionString);
        }
    }

    public static IAzureClientBuilder<QueueServiceClient, QueueClientOptions> AddQueueServiceClient(this AzureClientFactoryBuilder builder, string serviceUriOrConnectionString, bool preferMsi)
    {
        if (preferMsi && Uri.TryCreate(serviceUriOrConnectionString, UriKind.Absolute, out Uri? serviceUri))
        {
            return builder.AddQueueServiceClient(serviceUri);
        }
        else
        {
            return builder.AddQueueServiceClient(serviceUriOrConnectionString);
        }
    }
}