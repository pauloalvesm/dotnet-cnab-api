using CNAB.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CNAB.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CNABController : ControllerBase
{
    private readonly ICNABProcessingService _cnabProcessingService;
    private readonly ILogger<CNABController> _logger;

    public CNABController(ICNABProcessingService cnabProcessingService, ILogger<CNABController> logger)
    {
        _cnabProcessingService = cnabProcessingService;
        _logger = logger;
    }

    [HttpPost("upload-cnab-file")]
    public async Task<IActionResult> UploadCNABFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Invalid file.");
        }

        using var stream = new StreamReader(file.OpenReadStream());
        var lines = new List<string>();

        while (!stream.EndOfStream)
        {
            var line = await stream.ReadLineAsync();

            if (line.Length == 80)
            {
                line += " ";
            }
            lines.Add(line);
            _logger.LogInformation($"{line}");
        }

        var result = await _cnabProcessingService.ParseCNABAsync(lines);

        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        return Ok(new { message = "File processed successfully.", result.TotalProcessed });
    }
}