using BankApplication.Models.Dtos;
using BankApplication.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using BankApplication.Models.Dtos.AccoutDtos;
using BankApplication.Models.Dtos.AccountDtos;

namespace BankApplication.Managers.AccountManager
{

    public class AccountManager : IAccountManager
    {
        private BankDBContext _dbContext;
        private readonly AccountFactory _accountFactory;
        private readonly IMapper _mapper;
        public AccountManager(BankDBContext dBContext, AccountFactory accountFactory, IMapper mapper)
        {
            _dbContext = dBContext;
            _accountFactory = accountFactory;
            _mapper = mapper;
        }
        public async Task<Response<string>> TransferMoney(TransferMoney tranferData)
        {

            try
            {
                var from = await _dbContext.Accounts.FirstOrDefaultAsync((acc) => acc.AccNumber == long.Parse(tranferData.From));
                var to = await _dbContext.Accounts.FirstOrDefaultAsync((acc) => acc.AccNumber == long.Parse(tranferData.To));
                if (from == null || to == null)
                    return new Response<string> { Success = false, Message = "Customer not found" };
                if (from.IsActive == false) return new Response<string> { Message = "Account is inactive", Success = false };
                if (to.IsActive == false) return new Response<string> { Message = "Account is inactive", Success = false };
                if (tranferData.Amount <= 0)
                    return new Response<string> { Success = false, Message = $"Amount should be greater than 0" };
                if (from.AccBalance < tranferData.Amount)
                    return new Response<string> { Success = false, Message = "No enough balance" };
                if (tranferData.Amount > long.MaxValue)
                    return new Response<string> { Success = false, Message = "Amount must be less than" + long.MaxValue };

                from.AccBalance -= tranferData.Amount;
                to.AccBalance += tranferData.Amount;

                TransactionTbl t1 = new TransactionTbl();
                TransactionTbl t2 = new TransactionTbl();
                t1.TAmount = tranferData.Amount;
                t2.TAmount = tranferData.Amount;
                t1.TType = "transfer";
                t2.TType = "transfer";
                t1.AccNumber = from.AccNumber;
                t2.AccNumber = to.AccNumber;
                t1.TTime = DateTime.Now;
                t2.TTime = DateTime.Now;
                t1.Summary = $"Rs.{tranferData.Amount} sent to {to.AccNumber}";
                t2.Summary = $"Rs.{tranferData.Amount} recieved from {from.AccNumber}";
                await _dbContext.TransactionTbls.AddAsync(t1);
                await _dbContext.TransactionTbls.AddAsync(t2);
                await _dbContext.SaveChangesAsync();
                return new Response<string> { Success = true, Message = $"Rs {tranferData.Amount} transferred to {tranferData.To} from {tranferData.From}" };
            }
            catch (Exception ex)
            {
                return new Response<string> { Message = ex.Message, Success = false };
            }

        }


        public async Task<Response<string>> Deposite(TransactionDto depositeData)
        {
            try
            {
                long accNum = long.Parse(depositeData.AccountNumber);
                var account = await _dbContext.Accounts.FirstOrDefaultAsync((acc) => acc.AccNumber == accNum);

                if (account == null) return new Response<string> { Success = false, Message = "Customer not found" };
                if (account.IsActive == false) return new Response<string> { Message = "Account is inactive", Success = false };
                if (depositeData.Amount <= 0)
                    return new Response<string> { Success = false, Message = $"Amount should be greater than 0" };
                if (depositeData.Amount > long.MaxValue)
                    return new Response<string> { Success = false, Message = "Amount must be less than" + long.MaxValue };
                account.AccBalance += depositeData.Amount;
                TransactionTbl t1 = new TransactionTbl();

                t1.TAmount = depositeData.Amount;
                t1.TType = "deposite";
                t1.AccNumber = long.Parse(depositeData.AccountNumber);
                t1.TTime = DateTime.Now;
                t1.Summary = $"Rs.{depositeData.Amount} deposited to {depositeData.AccountNumber}";
                await _dbContext.TransactionTbls.AddAsync(t1);

                await _dbContext.SaveChangesAsync();
                return new Response<string> { Success = true, Message = $"Rs.{depositeData.Amount} successfully deposited to {depositeData.AccountNumber}" };
            }
            catch (Exception ex)
            {
                return new Response<string> { Success = false, Message = "error fetching data from database" };
            }
        }


        public async Task<Response<string>> Withdraw(TransactionDto withdrawData)
        {
            try
            {
                long accNum = long.Parse(withdrawData.AccountNumber);
                var account = await _dbContext.Accounts.FirstOrDefaultAsync((acc) => acc.AccNumber == accNum);
                if (account == null) return new Response<string> { Success = false, Message = "Account Not Found" };
                if (account.IsActive == false) return new Response<string> { Message = "Account is inactive", Success = false };
                if (withdrawData.Amount <= 0)
                    return new Response<string> { Success = false, Message = $"Amount should be greater than 0" };
                if (withdrawData.Amount > long.MaxValue)
                    return new Response<string> { Success = false, Message = "amount must be less than" + long.MaxValue };
                if (withdrawData.Amount > account.AccBalance)
                    return new Response<string> { Success = false, Message = "insufficient amount" };
                account.AccBalance -= withdrawData.Amount;
                TransactionTbl t1 = new TransactionTbl
                {
                    TAmount = withdrawData.Amount,
                    TType = "withdraw",
                    AccNumber = long.Parse(withdrawData.AccountNumber),
                    TTime = DateTime.Now,
                    Summary = $"Rs.{withdrawData.Amount} withdraw from {withdrawData.AccountNumber}"
                };
                await _dbContext.TransactionTbls.AddAsync(t1);
                await _dbContext.SaveChangesAsync();

                return new Response<string> { Success = true, Message = $"Rs.{withdrawData.Amount} Successfully withdraw from {withdrawData.AccountNumber}" };
            }
            catch (Exception ex)
            {
                return new Response<string> { Success = false, Message = "error fetching data from database" };
            }
        }

        public async Task<Response<string>> GetInterest(long accNumber)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync((acc) => acc.CustomerId == accNumber);
            if (account == null)
                return new Response<string> { Message = "invalid account type acccount type should be current or saving", Success = false };

            try
            {
                IAccount accountInstance = _accountFactory.CreateAccount(account.AccType);
                var interest = accountInstance.CalculateInterest(account.AccBalance);
                return new Response<string> { Message = "interest calculated sucessfully", Success = true, Data = interest.ToString() };
            }
            catch (Exception ex)
            {
                return new Response<string> { Message = "invalid account type acccount type should be current or saving" + ex, Success = false };
            }

        }


        public async Task<Response<List<AccountDto>>> GetAllAccounts()
        {
            try
            {
                var accounts = await _dbContext.Accounts.ToListAsync();
                if (accounts == null) return new Response<List<AccountDto>> { Message = "no acccounts found" };
                var res = accounts.Select(acc => _mapper.Map<AccountDto>(acc));
                return new Response<List<AccountDto>> { Data = res.ToList(), Success = true };
            }
            catch (Exception ex)
            {
                return new Response<List<AccountDto>> { Message = "error fetching data from database", Success = false };
            }
        }
        public async Task<Response<AccountDto>> GetAccountByCustId(long customerId)
        {
            try
            {
               
                var account = await _dbContext.Accounts.FirstOrDefaultAsync(acc => acc.CustomerId == customerId);
                if (account == null)
                    return new Response<AccountDto> { Message = "Account not found", Success = false };

                var accountDto = _mapper.Map<AccountDto>(account);
                return new Response<AccountDto> { Data = accountDto, Success = true };
            }
            catch (Exception ex)
            {
                return new Response<AccountDto> { Message = "Error fetching data from database", Success = false };
            }
        }

        public async Task<Response<string>> UpdateAccount(AccountDto account)
        {
            try
            {
                var existingAcc = await _dbContext.Accounts.FirstOrDefaultAsync((acc) => acc.AccNumber == account.AccNumber);
                if (existingAcc == null) return new Response<string> { Message = "Acc not found", Success = false };
                existingAcc.IsActive = account.IsActive;
                existingAcc.AccType = account.AccType;
                existingAcc.AccBalance = account.AccBalance;
                var res = await _dbContext.SaveChangesAsync();
                if (res > 0) return new Response<string> { Message = "acc updated", Success = true };
                return new Response<string> { Message = "exception occured while fetching data from database", Success = false };

            }
            catch (Exception ex)
            {
                return new Response<string> { Message = "exception occured while fetching data from database", Success = false };
            }
        }
    }
}
