using System.ComponentModel.DataAnnotations;

namespace CNAB.Application.DTOs;

public class TransactionDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Type is required")]
    [Range(1, 9, ErrorMessage = "Invalid transaction type")]
    public int Type { get; set; }

    [Required(ErrorMessage = "Occurrence Date is required")]
    public DateTime OccurrenceDate { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "CPF is required")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF must be 11 characters")]
    public string CPF { get; set; }

    [Required(ErrorMessage = "Card Number is required")]
    public string CardNumber { get; set; }

    [Required(ErrorMessage = "Time is required")]
    public TimeSpan Time { get; set; }

    [Required(ErrorMessage = "StoreId is required")]
    public Guid StoreId { get; set; }

    [Required(ErrorMessage = "StoreName is required")]
    [StringLength(100, ErrorMessage = "StoreName cannot exceed 100 characters")]
    public string StoreName { get; set; }

    [Required(ErrorMessage = "StoreOwnerName is required")]
    [StringLength(100, ErrorMessage = "StoreOwnerName cannot exceed 100 characters")]
    public string StoreOwnerName { get; set; }

    public TransactionDto() { }

    public TransactionDto(Guid id, int type, DateTime occurrenceDate, decimal amount, string cPF, string cardNumber, TimeSpan time, Guid storeId, string storeName, string storeOwnerName)
    {
        Id = id;
        Type = type;
        OccurrenceDate = occurrenceDate;
        Amount = amount;
        CPF = cPF;
        CardNumber = cardNumber;
        Time = time;
        StoreId = storeId;
        StoreName = storeName;
        StoreOwnerName = storeOwnerName;
    }
}