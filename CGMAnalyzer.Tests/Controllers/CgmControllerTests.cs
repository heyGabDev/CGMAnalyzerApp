using CGMAnalyzer.API.Controllers;
using CGMAnalyzer.API.Services.Interfaces;
using CGMAnalyzerCore.Modeles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.IO;
using System.Text;
using Xunit;

namespace CGMAnalyzer.Tests.Controllers
{
    public class CGMControllerTests
    {
        /// <summary>
        /// Test pour vérifier que l'importation d'un fichier CGM renvoie un résultat OK avec le chemin BMP converti.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ImportCgmFile_ShouldReturnOkResult_WhenCgmIsConverted()
        {
            // Arrange
            var mockImageService = new Mock<ICgmImageService>();
            mockImageService
                .Setup(s => s.GenerateAndSaveBmpAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("/images/test.bmp");
            var controller = new CGMController(mockImageService.Object);

            // Fournit juste une instance inutile
            var dummyLayerDetector = new Mock<ICGMLayerDetector>();

            // Faux fichier CGM simulé
            var fakeStream = new MemoryStream(Encoding.UTF8.GetBytes("fake cgm content"));
            var formFile = new FormFile(fakeStream, 0, fakeStream.Length, "Data", "test.cgm")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/octet-stream"
            };

            // Act
            var result = await controller.ImportCgmFile(new List<IFormFile> { formFile });

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsAssignableFrom<List<CGMResult>>(okResult.Value);
            Assert.Single(payload);
            Assert.Equal("test.cgm", payload[0].FileName);
            Assert.Equal("/images/test.bmp", payload[0].BmpPath);
        }


        /// <summary>
        /// Test pour vérifier que l'importation de plusieurs fichiers CGM renvoie un résultat OK avec les chemins BMP convertis.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ImportCgmFile_ShouldProcessMultipleFiles()
        {
            // Arrange
            var mockImageService = new Mock<ICgmImageService>();
            mockImageService
                .Setup(s => s.GenerateAndSaveBmpAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("/images/test.bmp");
            var controller = new CGMController(mockImageService.Object);
            var file1 = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("file1")), 0, 5, "Data", "file1.cgm");
            var file2 = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("file2")), 0, 5, "Data", "file2.cgm");

            // Act
            var result = await controller.ImportCgmFile(new List<IFormFile> { file1, file2 });

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsAssignableFrom<List<CGMResult>>(okResult.Value);

            Assert.Equal(2, payload.Count);
            Assert.Contains(payload, r => r.FileName == "file1.cgm");
            Assert.Contains(payload, r => r.FileName == "file2.cgm");
        }

        /// <summary>
        /// Test pour vérifier que l'importation sans fichier CGM renvoie une erreur.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ImportCgmFile_ShouldReturnBadRequest_WhenNoFilesProvided()
        {
            // Arrange
            var mockImageService = new Mock<ICgmImageService>();
            var controller = new CGMController(mockImageService.Object);

            // Act
            var result = await controller.ImportCgmFile(new List<IFormFile>());

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Aucun fichier reçu.", badRequest.Value);
        }

        /// <summary>
        /// Test pour vérifier que l'importation d'un fichier CGM échoue avec une erreur interne du serveur.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ImportCgmFile_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var mockImageService = new Mock<ICgmImageService>();
            mockImageService
                .Setup(s => s.GenerateAndSaveBmpAsync(It.IsAny<IFormFile>()))
                .ThrowsAsync(new Exception("Erreur simulée"));
            var controller = new CGMController(mockImageService.Object);
            var failStream = new MemoryStream(Encoding.UTF8.GetBytes("fail"));
            var file = new FormFile(failStream, 0, failStream.Length, "Data", "fail.cgm");

            // Act
            var result = await controller.ImportCgmFile(new List<IFormFile> { file });

            // Assert
            var errorResult = Assert.IsType<ObjectResult>(result);
            var message = errorResult.Value?.ToString() ?? ""; // null control
            Assert.Equal(500, errorResult.StatusCode);
            Assert.Contains("Erreur simulée", message);
        }
    }
}
