namespace BankApplication.Models.Dtos.UsersDto
{
    public class UserSignUpDto
    {
        public string UserName { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;
        public string? Email { get; set; }
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
