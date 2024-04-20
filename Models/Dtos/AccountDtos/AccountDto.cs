namespace BankApplication.Models.Dtos.AccoutDtos
{
    public class AccountDto
    {
        public long AccNumber { get; set; }
        public string AccType { get; set; } = null!;
        public long AccBalance { get; set; }
        public DateTime? AccCreatedAt { get; set; }
        public bool? IsActive { get; set; }
        public long CustomerId { get; set; }
    }
}
