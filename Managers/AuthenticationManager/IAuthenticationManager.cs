using BankApplication.Models.Dtos.UsersDto;
using BankApplication.Models.Dtos;
using BankApplication.Models;

namespace BankApplication.Managers.AuthenticationManager
{
    public interface IAuthenticationManager
    {
        Task<Response<User>> Login(UserLoginDto loginInformation);
        Task<Response<string>> Register(UserSignUpDto signUpInformation);
    }
}
