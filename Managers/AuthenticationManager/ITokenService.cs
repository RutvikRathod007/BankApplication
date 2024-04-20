using BankApplication.Models;

namespace BankApplication.Managers.AuthenticationManager
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
