namespace CNAB.Application.DTOs;

public class ParseResultDto
{
    public bool Success { get; set; }
    public int TotalProcessed { get; set; }
    public string Message { get; set; }
}