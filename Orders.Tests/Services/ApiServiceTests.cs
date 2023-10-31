using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Orders.Backend.Services;

namespace Orders.Tests.Services
{
    [TestClass]
    public class ApiServiceTests
    {
        private Mock<IConfiguration> _configurationMock = null!;
        private Mock<HttpMessageHandler> _handler = null!;
        private ApiService _apiService = null!;

        [TestInitialize]
        public void SetUp()
        {
            _configurationMock = new Mock<IConfiguration>();
            _handler = new Mock<HttpMessageHandler>();

            _configurationMock.Setup(c => c["CoutriesBackend:urlBase"]).Returns("http://localhost/");
            _configurationMock.Setup(c => c["CoutriesBackend:tokenName"]).Returns("Authorization");
            _configurationMock.Setup(c => c["CoutriesBackend:tokenValue"]).Returns("Bearer token");

            var client = new HttpClient(_handler.Object)
            {
                BaseAddress = new Uri("http://localhost/")
            };

            _apiService = new ApiService(_configurationMock.Object, client);
        }

        [TestMethod]
        public async Task GetAsync_SuccessfulRequest_ReturnsSuccessResponse()
        {
            // Arrange
            var expectedResponse = new { Data = "Test" };
            var json = JsonSerializer.Serialize(expectedResponse);

            _handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json),
                })
                .Verifiable();

            // Act
            var response = await _apiService.GetAsync<object>("service/", "controller");

            // Assert
            Assert.IsTrue(response.WasSuccess);
            Assert.IsNotNull(response.Result);
            Assert.AreEqual(expectedResponse.Data, ((JsonElement)response.Result).GetProperty("Data").GetString());
        }

        [TestMethod]
        public async Task GetAsync_UnsuccessfulRequest_ReturnsErrorResponse()
        {
            // Arrange
            var errorMessage = "Not Found";

            _handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent(errorMessage),
                })
                .Verifiable();

            // Act
            var response = await _apiService.GetAsync<object>("service/", "controller");

            // Assert
            Assert.IsFalse(response.WasSuccess);
            Assert.AreEqual(errorMessage, response.Message);
        }

        [TestMethod]
        public async Task GetAsync_ExceptionThrown_ReturnsErrorResponse()
        {
            // Arrange
            var exceptionMessage = "An error occurred";

            _handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new Exception(exceptionMessage))
                .Verifiable();

            // Act
            var response = await _apiService.GetAsync<object>("service/", "controller");

            // Assert
            Assert.IsFalse(response.WasSuccess);
            Assert.AreEqual(exceptionMessage, response.Message);
        }
    }
}