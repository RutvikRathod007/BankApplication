using BankApplication.Models;
using BankApplication.Models.Dtos;
using BankApplication.Models.Dtos.TransactionDtos;

namespace BankApplication.Managers.TransactionManager
{
    public interface ITransactionManager
    {
        Task<Response<List<TransactionTbl>>> GetAllTransactions();
        Task<Response<List<DisplayTransactionsDto>>> GetTransactionsByCustomerId(long custId);
        Task<Response<List<TransactionTbl>>> GetTransactionByTransactionType(long accNumber, string transactionType);
         

    }
}
