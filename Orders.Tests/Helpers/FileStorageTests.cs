using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using Orders.Backend.Helpers;

namespace Orders.Tests.Helpers
{
    [TestClass]
    public class FileStorageTests
    {
        [TestMethod]
        public async Task TestRemoveFileAsync()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["ConnectionStrings:AzureStorage"]).Returns("fake_connection_string");

            var blobClientMock = new Mock<BlobClient>();
            blobClientMock.Setup(c => c.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(true, Mock.Of<Response>()));

            var blobContainerClientMock = new Mock<IBlobContainerClient>();
            blobContainerClientMock.Setup(c => c.GetBlobClientAsync(It.IsAny<string>()))
                .ReturnsAsync(blobClientMock.Object);
            blobContainerClientMock.Setup(c => c.CreateIfNotExistsAsync())
                .Returns(Task.CompletedTask);

            var blobContainerClientFactoryMock = new Mock<IBlobContainerClientFactory>();
            blobContainerClientFactoryMock.Setup(f => f.CreateBlobContainerClient(It.IsAny<string>(), It.IsAny<string>())).Returns(blobContainerClientMock.Object);

            var fileStorage = new FileStorage(configurationMock.Object, blobContainerClientFactoryMock.Object);

            // Act
            await fileStorage.RemoveFileAsync("fake_path", "fake_container");

            // Assert
            blobClientMock.Verify(c => c.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task TestSaveFileAsync_Success()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["ConnectionStrings:AzureStorage"]).Returns("fake_connection_string");

            var blobClientMock = new Mock<BlobClient>();
            var blobContentInfoMock = new Mock<BlobContentInfo>();
            var responseMock = new Mock<Response<BlobContentInfo>>();
            responseMock.Setup(r => r.Value).Returns(blobContentInfoMock.Object);

            blobClientMock.Setup(c => c.UploadAsync(It.IsAny<Stream>(), true, default))
                .ReturnsAsync(responseMock.Object);
            blobClientMock.SetupGet(c => c.Uri)
                .Returns(new Uri("http://fake.blob.url"));

            var blobContainerClientMock = new Mock<IBlobContainerClient>();
            blobContainerClientMock.Setup(c => c.GetBlobClientAsync(It.IsAny<string>()))
                .ReturnsAsync(blobClientMock.Object);
            blobContainerClientMock.Setup(c => c.CreateIfNotExistsAsync())
                .Returns(Task.CompletedTask);
            blobContainerClientMock.Setup(c => c.SetAccessPolicyAsync(PublicAccessType.Blob))
                .Returns(Task.CompletedTask);

            var blobContainerClientFactoryMock = new Mock<IBlobContainerClientFactory>();
            blobContainerClientFactoryMock.Setup(f => f.CreateBlobContainerClient(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(blobContainerClientMock.Object);

            var fileStorage = new FileStorage(configurationMock.Object, blobContainerClientFactoryMock.Object);

            // Act
            var result = await fileStorage.SaveFileAsync(new byte[] { }, ".txt", "fake_container");

            // Assert
            Assert.AreEqual("http://fake.blob.url/", result);
        }
    }
}