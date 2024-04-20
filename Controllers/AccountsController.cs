using BankApplication.Managers.AccountManager;
using BankApplication.Models.Dtos;
using BankApplication.Models.Dtos.AccountDtos;
using BankApplication.Models.Dtos.AccoutDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApplication.Controllers
{
    [ApiController]
    // [Authorize(Roles ="admin")]
    [Authorize]
    [Route("api/[controller]")]
    public class AccountsController : Controller
    {

        private readonly AccountFactory _accountFactory;
        private readonly IAccountManager _accountManager;
        public AccountsController(AccountFactory accountFactory,IAccountManager accountManager)
        {
          _accountFactory= accountFactory;
            _accountManager= accountManager;
        }
        [HttpGet("getInterestRate{accountType}")]
        public ActionResult GetInterestRate([FromRoute]string accountType)
        {
            try
            {
                IAccount accountInstance = _accountFactory.CreateAccount(accountType.ToLower());
                var interestRate = accountInstance.GetInterest();
                return Ok(new Response<string> { Success = true,Data= $"interest rate is:{interestRate}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response<string> { Message = "invalid account type acccount type should be current or saving", Success = false });

            }
        }

        [HttpGet("getInterest/{accNumber:long}")]
        public async Task<ActionResult> GetInterest(long accNumber)
        {

            var res = await _accountManager.GetInterest(accNumber);
            if (res.Success)
            {
                return Ok(res.Message);
            }
            return BadRequest(res.Message);

        }

        [HttpPost("transferMoney")]
        public async Task<ActionResult> TransferMoney([FromBody] TransferMoney tranferData)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var res = await _accountManager.TransferMoney(tranferData);
            if (res.Success) return Ok(res);
            return BadRequest(res);
        }

        [HttpPost("depositeMoney")]
        public async Task<ActionResult> Deposite([FromBody] TransactionDto depositeData)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var res = await _accountManager.Deposite(depositeData);
            if (res.Success) return Ok(res);
            return BadRequest(res);
        }
        [HttpPost("withdrawMoney")]
        public async Task<ActionResult> Withdraw([FromBody] TransactionDto withdrawData)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var res = await _accountManager.Withdraw(withdrawData);
            if (res.Success) return Ok(res);
            return BadRequest(res);
        }
        [HttpGet("getAllAccounts")]
        public async Task<IActionResult> GetAllAccounts()
        {
            var res = await _accountManager.GetAllAccounts();
            if (res.Success) return Ok(res);
            return NotFound(res);
        }
        [HttpGet("getAccountByCustId/{custId}")]
        public async Task<IActionResult> GetAllAccounts(long custId)
        {
            var res = await _accountManager.GetAccountByCustId(custId);
            if (res.Success) return Ok(res);
            return NotFound(res);
        }


        [HttpPut("updateAccount")]
        public async Task<IActionResult> UpdateAccount([FromBody] AccountDto account)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var res = await _accountManager.UpdateAccount(account);
            if (res.Success) return Ok(res);
            return BadRequest(res);
        }
    } 
}
