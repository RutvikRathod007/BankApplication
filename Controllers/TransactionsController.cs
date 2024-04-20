using BankApplication.Managers.TransactionManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BankApplication.Controllers
{
    [ApiController]
    // [Authorize(Roles = "admin")]
    [Authorize]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private ITransactionManager _transactionManager;
  
        public TransactionsController(ITransactionManager transactionManager)
        {
            _transactionManager = transactionManager;
        }

        [HttpGet("getAllTransactions")]
        public async Task<IActionResult> GetAllTransactions()
        {
            var res=await _transactionManager.GetAllTransactions();
            if (res.Success) return Ok(res);
            return NotFound(res);
        
        }
        [HttpGet("getTransactionsByCustomerId/{custId}")]
        public  async Task<IActionResult> GetTransactions(long custId)
        {
            var res = await _transactionManager.GetTransactionsByCustomerId(custId);
            if (res.Success) return Ok(res);
            return NotFound(res);
        }
        [HttpGet("getTransactionsByTransactionType")]
       public async Task<IActionResult> GetTransactionByTransactionType(long accNumber, string transactionType)
        {
            var res = await _transactionManager.GetTransactionByTransactionType(accNumber,transactionType);
            if (res.Success) return Ok(res);
            return NotFound(res);
        }




    }
}
