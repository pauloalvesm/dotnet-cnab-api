using System.ComponentModel.DataAnnotations;

namespace CNAB.Application.DTOs;

public class StoreInputDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Owner Name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Owner Name must be between 3 and 100 characters")]
    public string OwnerName { get; set; }
}