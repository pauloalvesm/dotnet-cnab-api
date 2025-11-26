using System.Text;
using CNAB.Application.DTOs;
using CNAB.Application.Interfaces;
using CNAB.WebAPI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace CNAB.WebAPI.Test.Controllers;

public class CNABControllerTest
{
    private readonly Mock<ICNABProcessingService> _mockCnabProcessingService;
    private readonly Mock<ILogger<CNABController>> _mockLogger;
    private readonly CNABController _cnabController;

    public CNABControllerTest()
    {
        _mockCnabProcessingService = new Mock<ICNABProcessingService>();
        _mockLogger = new Mock<ILogger<CNABController>>();
        _cnabController = new CNABController(_mockCnabProcessingService.Object, _mockLogger.Object);
    }

    [Fact(DisplayName = "UploadCNABFile - Should return Ok when file is valid and processed successfully")]
    public async Task UploadCNABFile_ShouldReturnOk_WhenFileIsValidAndProcessedSuccessfully()
    {
        // Arrange
        var cnabContent = new List<string>
        {
            "3201903010000014200096206760174753****3153153453JOÃO MACEDO        BAR DO JOÃO         ",
            "5201903010000013200556418150633123****7687145607MARIA JOSEFINALOJA DO Ó - MATRIZ",
            "1201903010000015200096206760171234****7890233000JOÃO MACEDO        BAR DO JOÃO         "
        };

        var expectedLinesForService = new List<string>
        {
            cnabContent[0],
            cnabContent[1] + " ",
            cnabContent[2]
        };

        var memoryStream = new MemoryStream();
        using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
        {
            foreach (var line in cnabContent)
            {
                await streamWriter.WriteLineAsync(line);
            }
        }
        memoryStream.Position = 0;

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(memoryStream.Length);
        mockFile.Setup(f => f.OpenReadStream()).Returns(memoryStream);
        mockFile.Setup(f => f.FileName).Returns("valid_cnab.txt");
        mockFile.Setup(f => f.ContentType).Returns("text/plain");

        var parseResult = new ParseResultDto { Success = true, TotalProcessed = cnabContent.Count, Message = "File processed successfully." };
        _mockCnabProcessingService.Setup(s => s.ParseCNABAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(parseResult);

        // Act
        var result = await _cnabController.UploadCNABFile(mockFile.Object);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;

        var jsonResultContent = JsonConvert.SerializeObject(okResult.Value);

        var controllerReturnSchema = new { message = "", TotalProcessed = 0 };
        var parsedResult = JsonConvert.DeserializeAnonymousType(jsonResultContent, controllerReturnSchema);

        parsedResult.Should().NotBeNull();
        parsedResult.message.Should().Be("File processed successfully.");
        parsedResult.TotalProcessed.Should().Be(cnabContent.Count);
        _mockCnabProcessingService.Verify(s => s.ParseCNABAsync(It.Is<List<string>>(lines =>
            lines.Count == expectedLinesForService.Count &&
            lines.SequenceEqual(expectedLinesForService)
        )), Times.Once);

        foreach (var line in expectedLinesForService)
        {
            _mockLogger.Verify(x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(line)),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }
    }


    [Fact(DisplayName = "UploadCNABFile - Should return BadRequest when file is null")]
    public async Task UploadCNABFile_ShouldReturnBadRequest_WhenFileIsNull()
    {
        // Arrange
        IFormFile file = null;

        // Act
        var result = await _cnabController.UploadCNABFile(file);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = (BadRequestObjectResult)result;
        badRequestResult.Value.Should().Be("Invalid file.");

        _mockCnabProcessingService.Verify(s => s.ParseCNABAsync(It.IsAny<IEnumerable<string>>()), Times.Never);
    }

    [Fact(DisplayName = "UploadCNABFile - Should return BadRequest when file is empty")]
    public async Task UploadCNABFile_ShouldReturnBadRequest_WhenFileIsEmpty()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(0); // Arquivo vazio
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

        // Act
        var result = await _cnabController.UploadCNABFile(mockFile.Object);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = (BadRequestObjectResult)result;
        badRequestResult.Value.Should().Be("Invalid file.");

        _mockCnabProcessingService.Verify(s => s.ParseCNABAsync(It.IsAny<IEnumerable<string>>()), Times.Never);
    }

    [Fact(DisplayName = "UploadCNABFile - Should return BadRequest when CNAB processing fails")]
    public async Task UploadCNABFile_ShouldReturnBadRequest_WhenCnabProcessingFails()
    {
        // Arrange
        var cnabContent = new List<string>
        {
            "3201903010000014200096206760174753****3153153453JOÃO MACEDO        BAR DO JOÃO         "
        };
        var memoryStream = new MemoryStream();
        using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
        {
            foreach (var line in cnabContent)
            {
                await streamWriter.WriteLineAsync(line);
            }
        }
        memoryStream.Position = 0;

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(memoryStream.Length);
        mockFile.Setup(f => f.OpenReadStream()).Returns(memoryStream);
        mockFile.Setup(f => f.FileName).Returns("invalid_cnab_format.txt");
        mockFile.Setup(f => f.ContentType).Returns("text/plain");

        var parseResult = new ParseResultDto { Success = false, TotalProcessed = 0, Message = "Invalid line format detected." };
        _mockCnabProcessingService.Setup(s => s.ParseCNABAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(parseResult);

        // Act
        var result = await _cnabController.UploadCNABFile(mockFile.Object);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = (BadRequestObjectResult)result;
        badRequestResult.Value.Should().Be("Invalid line format detected.");

        _mockCnabProcessingService.Verify(s => s.ParseCNABAsync(It.IsAny<IEnumerable<string>>()), Times.Once);

        _mockLogger.Verify(x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(cnabContent[0])),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
    }
}