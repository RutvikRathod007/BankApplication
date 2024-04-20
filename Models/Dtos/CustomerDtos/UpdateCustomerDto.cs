namespace BankApplication.Models.Dtos.CustomerDtos
{
    public class UpdateCustomerDto
    {
        public long CustomerId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime Dob { get; set; }
        public string MobileNumber { get; set; } = null!;
        public string City { get; set; } = null!;
        public IFormFile? CustomerImage { get; set; }
    }
}
