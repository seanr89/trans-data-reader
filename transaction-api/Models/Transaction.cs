using System;
using System.ComponentModel.DataAnnotations;

namespace transaction_api.Models;

public class Transaction
{
    [Key]
    public Guid TransactionId { get; set; }
    public Guid AccountId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string TransactionCurrency { get; set; } = string.Empty;
    public decimal TransactionAmount { get; set; }
    public string TransactionNarrative { get; set; } = string.Empty;
    public string ReversalIndicator { get; set; } = string.Empty;
    public string MID { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty;
    public string MCC { get; set; } = string.Empty;
    public string Narrative { get; set; } = string.Empty;
}
