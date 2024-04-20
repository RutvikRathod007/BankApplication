using BankApplication.Models;
using BankApplication.Models.Dtos;
using BankApplication.Models.Dtos.CustomerDtos;
using BankApplication.Models.Dtos.UsersDto;

namespace BankApplication.Managers.CustomerManager
{
    public interface ICustomerManager
    {
        Task<Response<string>> CreateNewAccount(OpenCustomerAccount custData);
        Task<Response<List<CustomerDto>>> GetAllCustomers();
        Task<Response<Customer>> GetCustomerByAccNumber(long accNumber);
        Task<Response<string>> RemoveCustomer(long accNumber);
        Task<Response<byte[]>> GetCustomerImage(long accNumber);
        Task<Response<string>> UpdateCustomer(UpdateCustomerDto updateCustomer);
        Response<List<Customer>> GetCustomerByPagination(int pageSize, int pageNumber);
        Response<List<Customer>> FilterCustomerByName(int pageSize, int pageNumber,string searchText);
    }
}
