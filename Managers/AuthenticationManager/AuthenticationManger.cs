using BankApplication.Models.Dtos.UsersDto;
using BankApplication.Models.Dtos;
using BankApplication.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Text;
using System.Security.Cryptography;

namespace BankApplication.Managers.AuthenticationManager
{
    public class AuthenticationManger:IAuthenticationManager
    {
        
        private BankDBContext _dbContext;
        private readonly IMapper _mapper;

     
            public AuthenticationManger(BankDBContext dBContext, IMapper mapper)
            {
                _dbContext = dBContext;
                _mapper = mapper;

            }
        
        public async Task<Response<User>> Login(UserLoginDto loginInformation)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.MobileNumber == loginInformation.MobileNumber && user.Password == loginInformation.Password);
            if (user == null) return new Response<User> { Message = "Invalid login credentials", Success = false };
            return new Response<User> { Success = true, Message = "Login Success", Data = user };
        }
        public string GenerateSalt(int length)
        {
            byte[] salt = new byte[length];

           var rng= RandomNumberGenerator.GetBytes(length);
            return Convert.ToBase64String(salt);
            
        }
        public async Task<Response<string>> Register(UserSignUpDto signupInfo)
        {
            try
            {
                var salt = GenerateSalt(16);
                var sha256 = SHA256.Create();
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(signupInfo.Password + salt));
                signupInfo.Password = Convert.ToBase64String(hashedBytes);

                var user = _mapper.Map<User>(signupInfo);
                user.Salt = salt;
                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();
                return new Response<string> { Message = "Register Success", Success = true };
            }
            catch(Exception ex)
            {
                return new Response<string> { Message = "Register Error"+ex, Success = true };
            }
           
        }

    }
}
