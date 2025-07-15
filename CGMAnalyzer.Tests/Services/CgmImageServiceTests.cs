using CGMAnalyzer.API.Services;
using CGMAnalyzerCore.Convert;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using System.IO;
using System.Text;
using Xunit;

namespace CGMAnalyzer.Tests.Services
{
    public class CgmImageServiceTests
    {
        /// <summary>
        /// Test to verify that GenerateAndSaveBmpAsync saves the file and returns the correct path.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GenerateAndSaveBmpAsync_ShouldSaveFileAndReturnPath()
        {
            // Arrange
            var dummyBytes = Encoding.UTF8.GetBytes("fake image");
            var mockConverter = new Mock<ICgmConverter>();
            mockConverter
                .Setup(c => c.ConvertToBmpBytesAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(dummyBytes);

            var mockEnv = new Mock<IWebHostEnvironment>();
            mockEnv.Setup(e => e.WebRootPath).Returns("wwwroot_test");

            var service = new CgmImageService(mockConverter.Object,mockEnv.Object);

            var content = new MemoryStream(Encoding.UTF8.GetBytes("fake cgm"));
            var file = new FormFile(content, 0, content.Length, "Data", "test.cgm");

            // Act
            var path = await service.GenerateAndSaveBmpAsync(file);

            // Assert
            Assert.StartsWith("/images/", path);
            Assert.EndsWith(".bmp", path);
        }

        /// <summary>
        /// Test to verify that GenerateAndSaveBmpAsync throws an exception when the converter fails.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GenerateAndSaveBmpAsync_ShouldThrow_WhenConverterFails()
        {
            // Arrange
            var mockConverter = new Mock<ICgmConverter>();
            mockConverter
                .Setup(c => c.ConvertToBmpBytesAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Conversion échouée"));

            var mockEnv = new Mock<IWebHostEnvironment>();
            mockEnv.Setup(e => e.WebRootPath).Returns("wwwroot_test");

            var service = new CgmImageService(mockConverter.Object, mockEnv.Object);

            var content = new MemoryStream(Encoding.UTF8.GetBytes("bad data"));
            var file = new FormFile(content, 0, content.Length, "Data", "fail.cgm");

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                service.GenerateAndSaveBmpAsync(file));

            Assert.Equal("Conversion échouée", ex.Message);
        }

    }
}
