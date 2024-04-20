namespace BankApplication.Models.Dtos.CustomerDtos
{

    public class CustomerDto
    {

        public long CustomerId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public long AccountNumber { get; set; }
        public DateTime Dob { get; set; }
        public string MobileNumber { get; set; } = null!;
        public string City { get; set; } = null!;


    }
}
