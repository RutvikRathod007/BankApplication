namespace BankApplication.Models.Dtos.AccountDtos
{
    public class TransactionDto
    {
        public string AccountNumber { get; set; } = null!;
        public long Amount { get; set; }
    }
}
