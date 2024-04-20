namespace BankApplication.Models.Dtos.TransactionDtos
{
    public class DisplayTransactionsDto
    {
        public long TId { get; set; }
        public long AccNumber { get; set; }
        public string TType { get; set; } = null!;
        public long TAmount { get; set; }
        public DateTime TTime { get; set; }
        public string Summary { get; set; } = null!;
   
    }
}
