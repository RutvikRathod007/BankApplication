
using AutoMapper;
using BankApplication.Models;
using BankApplication.Models.Dtos;
using BankApplication.Models.Dtos.AccoutDtos;
using BankApplication.Models.Dtos.CustomerDtos;
using BankApplication.Models.Dtos.TransactionDtos;
using BankApplication.Models.Dtos.UsersDto;

namespace BankApplication
{
    public class AutoMapperProfiler:Profile
    {
        public AutoMapperProfiler()
        {
           
            CreateMap<OpenCustomerAccount, CustomerDto>().ReverseMap();
            CreateMap<Account, CustomerDto>().ReverseMap();
            CreateMap<Customer,PatchCustModel>().ReverseMap();
            CreateMap<Account, AccountDto>();
            CreateMap<Customer, UpdateCustomerDto>().ReverseMap();
            CreateMap<UserSignUpDto,User>().ReverseMap();
            CreateMap<DisplayTransactionsDto, TransactionTbl>().ReverseMap();
        }
    }
}
