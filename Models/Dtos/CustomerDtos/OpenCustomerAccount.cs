using Microsoft.Extensions.FileProviders;
using System.ComponentModel.DataAnnotations;

namespace BankApplication.Models.Dtos.CustomerDtos
{

    public class OpenCustomerAccount
    {

        public IFormFile CustomerImage { get; set; } = null!;

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime Dob { get; set; }
        public string MobileNumber { get; set; } = null!;
        public string City { get; set; } = null!;
        public string AccountType { get; set; } = null!;
        public long Amount { get; set; }
    }
}
