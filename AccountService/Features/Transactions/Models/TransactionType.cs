namespace AccountService.Features.Transactions.Models;

public enum TransactionType
{
    /// <summary>
    /// Зачисление
    /// </summary>
    Credit,
    
    /// <summary>
    /// Списание
    /// </summary>
    Debit
}