using AutoMapper;
using BankApplication.Models;
using BankApplication.Models.Dtos;
using BankApplication.Models.Dtos.TransactionDtos;
using Microsoft.EntityFrameworkCore;

namespace BankApplication.Managers.TransactionManager
{
    public class TransactionManager : ITransactionManager
    {
        private BankDBContext _dbContext;
        private IMapper _mapper;
        public TransactionManager(BankDBContext dBContext,IMapper mapper)
        {
            _dbContext = dBContext;
            _mapper = mapper;
        }
        public async Task<Response<List<TransactionTbl>>> GetAllTransactions()
        {
            try
            {

                var transactions = await _dbContext.TransactionTbls.ToListAsync();
                if (transactions.Count == 0) return new Response<List<TransactionTbl>> { Success = false, Message = "no transactions" };
                return new Response<List<TransactionTbl>> { Data = transactions.ToList(), Success = true };

            }
            catch (Exception ex)
            {
                return new Response<List<TransactionTbl>> { Success = false, Message = "Error while fetching data from database" + ex };

            }
        }


        public async Task<Response<List<TransactionTbl>>> GetTransactionByTransactionType(long accNumber, string transactionType)
        {
            try
            {
                var account = await _dbContext.TransactionTbls.FirstOrDefaultAsync(acc => acc.AccNumber == accNumber);
                if (account == null) return new Response<List<TransactionTbl>> { Success = false, Message = "account not found" };
                var transactions = await _dbContext.TransactionTbls.Where(t => t.AccNumber == accNumber && t.TType == transactionType).ToListAsync();
                if (transactions.Count == 0) return new Response<List<TransactionTbl>> { Success = false, Message = "no transactions" };
                return new Response<List<TransactionTbl>> { Data = transactions, Success = true };

            }
            catch (Exception ex)
            {
                return new Response<List<TransactionTbl>> { Success = false, Message = "Error while fetching data from database" + ex };

            }
        }

        public async Task<Response<List<DisplayTransactionsDto>>> GetTransactionsByCustomerId(long custId)
        {
            try
            {

                var customer = await _dbContext.Customers.FirstOrDefaultAsync((cust) => cust.CustomerId == custId);
                var account = await _dbContext.Accounts.FirstOrDefaultAsync((acc) => acc.CustomerId == custId);
                if (customer == null || account==null) return new Response<List<DisplayTransactionsDto>> { Message = "Customer not found", Success = false };
            
                    var res = await _dbContext.TransactionTbls.Where((t) => t.AccNumber == account.AccNumber).ToListAsync();
                var transactions = res.Select(t=>_mapper.Map<DisplayTransactionsDto>(t)).ToList();
                    if (transactions.Count == 0) return new Response<List<DisplayTransactionsDto>> { Message = "No Transactions", Success = true };
                    return new Response<List<DisplayTransactionsDto>> { Data = transactions, Success = true };

         
            }
            catch(Exception ex) {
                return new Response<List<DisplayTransactionsDto>> { Message = "error fetching data from database", Success = false };
            }

        }
    }
}
