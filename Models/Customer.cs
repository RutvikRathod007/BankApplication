using System;
using System.Collections.Generic;

namespace BankApplication.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Accounts = new HashSet<Account>();
        }

        public long CustomerId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime Dob { get; set; }
        public string MobileNumber { get; set; } = null!;
        public string City { get; set; } = null!;
        public string CustomerImage { get; set; } = null!;

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
