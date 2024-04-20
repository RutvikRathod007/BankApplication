namespace BankApplication.Models.Dtos.AccoutDtos
{
    public class TransferMoney
    {
        public string From { get; set; } = null!;
        public string To { get; set; } = null!;
        public long Amount { get; set; }
    }
}
