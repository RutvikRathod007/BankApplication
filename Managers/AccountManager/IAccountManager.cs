using BankApplication.Models.Dtos;
using BankApplication.Models.Dtos.AccountDtos;
using BankApplication.Models.Dtos.AccoutDtos;

namespace BankApplication.Managers.AccountManager
{
    public interface IAccountManager
    {

        Task<Response<string>> GetInterest(long accNumber);
        Task<Response<string>> TransferMoney(TransferMoney tranferData);
        Task<Response<string>> Deposite(TransactionDto depositeData);
        Task<Response<string>> Withdraw(TransactionDto withdrawData);
        Task<Response<List<AccountDto>>> GetAllAccounts();
        Task<Response<AccountDto>> GetAccountByCustId(long custId);
        Task<Response<string>> UpdateAccount(AccountDto account);

    }
}
