using AutoMapper;
using BankApplication.Managers.CustomerManager;
using BankApplication.Models;
using BankApplication.Models.Dtos;
using BankApplication.Models.Dtos.CustomerDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;


namespace BankApplication.Controllers
{
    [ApiController]
    // [Authorize(Roles ="admin")]
    [Authorize]
    [Route("api/[controller]")]
    public class CustomerController : Controller
    {
        private ICustomerManager _customerManager;
        private IMapper _mapper;
     
        public CustomerController(IMapper mapper, ICustomerManager customerManager)
        {
            _customerManager = customerManager;
            _mapper = mapper;
        }
        [HttpGet("getAllCustomer")]
        public async Task<IActionResult> GetAllCustomers()
        {

            var res = await _customerManager.GetAllCustomers();
            if (res.Success) return Ok(res);
            return NotFound(res);

        }

        [HttpPost("addCustomer")]
        public async Task<IActionResult> CreateNewAccount([FromForm] OpenCustomerAccount customerData)
        {
           
            if(!ModelState.IsValid) return BadRequest(ModelState);
            var res = await _customerManager.CreateNewAccount(customerData);
            if (res.Success) return Ok(res);
            return BadRequest(res.Message);

        }

        [HttpGet("getCustomerByAccountNumber/{accNumber:long}")]
        public async Task<IActionResult> GetCustomerByAccNumber([FromRoute] long accNumber)
        {

            var res = await _customerManager.GetCustomerByAccNumber(accNumber);
            if (res.Success) return Ok(res);
            return NotFound(res);

        }
       

        [HttpDelete("removeCustomer/{custId}")]
        public async Task<ActionResult> RemoveCustomer(long accNumber)
        {
            var res = await _customerManager.RemoveCustomer(accNumber);
            if (res.Success) return Ok("Customer account removed");
            return NotFound("Customer not exists..");

        }


        [HttpPatch("updateCustomer/{custId:int}")]
        public ActionResult UpdateCustomerPartial([FromRoute] int custId, [FromBody] JsonPatchDocument<PatchCustModel> customerPatch)
        {
            using (var context = new BankDBContext())
            {
                var customerEntity = context.Customers.FirstOrDefault(c => c.CustomerId == custId);
                if (customerEntity == null)
                {
                    return BadRequest("Customer not found");
                }

                var customerDto = _mapper.Map<PatchCustModel>(customerEntity);
                customerPatch.ApplyTo(customerDto); // Apply patch to DTO

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _mapper.Map(customerDto, customerEntity); // Map patched DTO back to entity

                try
                {
                    context.SaveChanges(); // Save changes to the database
                    return Ok("Updated");
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it appropriately
                    return StatusCode(500, "An error occurred while updating the customer." + ex);
                }
            }
        }


        [HttpGet("getCustomerImage/{custId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetImage(long custId)
        {

            var res = await _customerManager.GetCustomerImage(custId);
            if (res.Success && res.Data != null) { return File(res.Data, "image/jpeg"); }
            return NotFound();

        }

        [HttpPut("updateCustomer")]
        public async Task<IActionResult> UpdateCustomer([FromForm] UpdateCustomerDto customer)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var res = await _customerManager.UpdateCustomer(customer);
            if (res.Success) return Ok(res);
            return BadRequest(res);


        }
      
        [HttpGet("getCustomerByPagination")]
        public IActionResult GetCustomerByPagination(int pageNumber, int pageSize)
        {
            return Ok(_customerManager.GetCustomerByPagination(pageNumber,pageSize));
        }
        [HttpGet("filterCustomerByName")]
        public IActionResult GetCustomerByPagination( int pageSize, int pageNumber, string searchText)
        {
            return Ok(_customerManager.FilterCustomerByName(pageSize, pageNumber, searchText));
        }

    }

 
}
