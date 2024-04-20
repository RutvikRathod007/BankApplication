using System;
using System.Collections.Generic;

namespace BankApplication.Models
{
    public partial class User
    {
        public long UserId { get; set; }
        public string Username { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;
        public string? Email { get; set; }
        public string Password { get; set; } = null!;
        public string Salt { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
