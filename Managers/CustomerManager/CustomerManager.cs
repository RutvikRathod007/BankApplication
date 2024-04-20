using AutoMapper;
using BankApplication.Models;
using BankApplication.Models.Dtos;
using BankApplication.Models.Dtos.CustomerDtos;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BankApplication.Managers.CustomerManager
{
    public class CustomerManager : ICustomerManager
    {


        private BankDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly string imageUploadPath = @"C:\\Users\\Rutvik.Rathod\\source\\repos\\BankApplication\\BankApplication\\UserUploads\\UserImages\\";
        public CustomerManager(BankDBContext dBContext, IMapper mapper)
        {
            _dbContext = dBContext;
            _mapper = mapper;

        }


        public async Task<Response<List<CustomerDto>>> GetAllCustomers()
        {
            try
            {

                var customers = await _dbContext.Customers.ToListAsync();
                var accounts = await _dbContext.Accounts.ToListAsync();
                if (customers.Count == 0) return new Response<List<CustomerDto>> { Success = false, Message = "no customers found" };
                var res = customers.Join(accounts, cust => cust.CustomerId, acc => acc.CustomerId,

                 (cust, acc) =>
                 new CustomerDto
                 {
                     CustomerId = cust.CustomerId,
                     FirstName = cust.FirstName,
                     LastName = cust.LastName,
                     Dob = cust.Dob,
                     MobileNumber = cust.MobileNumber,
                     City = cust.City,
                     AccountNumber = acc.AccNumber,


                 });
                return new Response<List<CustomerDto>> { Data = res.ToList(), Success = true };

            }
            catch (Exception ex)
            {
                return new Response<List<CustomerDto>> { Success = false, Message = "Error while fetching data from database" + ex };

            }
        }
        public async Task<Response<string>> CreateNewAccount(OpenCustomerAccount custData)
        {


            //if (custData.FirstName.Length < 3) return new Response<string> { Message = "FirstName should be of atleast 3 characters", Success = false };
            //Regex re = new Regex("/^[a-zA-Z]+$/");
            //if (!re.IsMatch(custData.FirstName)) return new Response<string> { Message = "FirstName should contain only alphabets", Success = false };
            var resp = await UploadImage(custData.CustomerImage);
            try
            {



                if (resp.Success)
                {
                    var customerAccountNumberParam = new SqlParameter("@acc_number", SqlDbType.BigInt)
                    {
                        Direction = ParameterDirection.Output
                    };

                    var res = await _dbContext.Database.ExecuteSqlRawAsync("sp_insert_customer @cust_first_name, @cust_last_name, @cust_dob, @cust_mobile_number, @cust_city,@cust_image, @acc_type, @acc_balance, @acc_number OUTPUT",
                        new SqlParameter("@cust_first_name", custData.FirstName),
                        new SqlParameter("@cust_last_name", custData.LastName),
                        new SqlParameter("@cust_dob", custData.Dob),
                        new SqlParameter("@cust_mobile_number", custData.MobileNumber),
                        new SqlParameter("@cust_city", custData.City),
                        new SqlParameter("@cust_image", resp.Message),
                        new SqlParameter("@acc_type", custData.AccountType),
                        new SqlParameter("@acc_balance", custData.Amount),
                        customerAccountNumberParam);


                    long accountNumber = (long)customerAccountNumberParam.Value;
                    TransactionTbl transaction=new TransactionTbl();
                    transaction.AccNumber = accountNumber;
                    transaction.Summary = $"Rs.{custData.Amount} Deposited";
                    transaction.TAmount = custData.Amount;
                    transaction.TTime = DateTime.Now;
                    transaction.TType = "deposite";
                    await _dbContext.TransactionTbls.AddAsync(transaction);
                    await _dbContext.SaveChangesAsync();
                    return new Response<string> { Success = true, Message = $"account created with account number {accountNumber}" };
                }
                else
                {
                    return new Response<string> { Success = false, Message = "image upload error" };
                }

            }
            catch (Exception ex)
            {
                DeleteImage(resp.Message);
                Console.WriteLine(ex.Message);
                return new Response<string> { Success = false, Message = "could not create account..validation error occured" };
            }
        }

        public async Task<Response<Customer>> GetCustomerByAccNumber(long accNumber)
        {
            try
            {
                var account = await _dbContext.Accounts.FirstOrDefaultAsync(acc => acc.AccNumber == accNumber);
                if (account == null)
                {
                    return new Response<Customer> { Success = false, Message = "customer not found" };
                }
                var customer = await _dbContext.Customers.FindAsync(account.CustomerId);
                return new Response<Customer> { Data = customer, Success = true };
                //var customerDto=_mapper.Map<CustomerDto>(customer);
            }
            catch (Exception ex)
            {
                return new Response<Customer> { Success = false, Message = "Error fetching data from database" + ex };
            }

        }


        public async Task<Response<string>> RemoveCustomer(long accNumber)
        {
            try
            {
                var existingAccount = await _dbContext.Accounts.FirstOrDefaultAsync(acc => acc.AccNumber == accNumber);
                var existingCustomer = await _dbContext.Customers.FindAsync(existingAccount?.CustomerId);
                if (existingAccount == null || existingCustomer == null)
                    return new Response<string> { Message = "Customer not found", Success = false };


                _dbContext.Accounts.Remove(existingAccount);
                _dbContext.Customers.Remove(existingCustomer);
                await _dbContext.SaveChangesAsync();
                return new Response<string> { Message = "Customer removed", Success = true };
            }
            catch (Exception ex)
            {
                return new Response<string> { Message = "Error fetching data from database" + ex, Success = false };
            }

        }




        public async Task<Response<string>> UploadImage(IFormFile file)
        {
            if (file == null) return new Response<string> { Message = "image required", Success = false };
            try
            {


                if (file.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(imageUploadPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    return new Response<string> { Success = true, Message = filePath };
                }
                return new Response<string> { Success = false, Message = "File length must be greater than zero" };
            }

            catch (Exception ex)
            {
                return new Response<string> { Success = false, Message = "Error while uploadig image to server" + ex };
            }
        }

        public async Task<Response<byte[]>> GetCustomerImage(long custId)
        {
            try
            {
                byte[] b = { };
                var customer = await _dbContext.Customers.FirstOrDefaultAsync(cust => cust.CustomerId == custId);
                if (customer == null) return new Response<byte[]> { Success = false, Message = "customer not dound" };
            
                if (customer != null)
                    b = await File.ReadAllBytesAsync(customer.CustomerImage);
                if (b.Length == 0)
                    return new Response<byte[]> { Success = false, Message = "image not found" };
                return new Response<byte[]> { Data = b, Success = true };
            }
            catch (Exception ex)
            {
                throw new Exception("image fetch error", ex);
            }
        }
        public Response<string> DeleteImage(string imagePath)
        {
            try
            {




                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                    return new Response<string> { Success = true, Message = "file deleted" };
                }


                return new Response<string> { Success = true };
            }

            catch (Exception ex)
            {
                return new Response<string> { Success = false, Message = "Error while deleting image from server" + ex };
            }
        }
        public async Task<Response<string>> UpdateCustomer(UpdateCustomerDto customer)
        {
            try
            {
                var existingCust = await _dbContext.Customers.FirstOrDefaultAsync((cust) => cust.CustomerId == customer.CustomerId);
                if (existingCust == null) return new Response<string> { Message = "Customer not found", Success = false };
                var customerAccount = await _dbContext.Accounts.FirstOrDefaultAsync(acc => acc.CustomerId == existingCust.CustomerId);
                if (customerAccount?.IsActive == false) return new Response<string> { Success = false, Message = "customer is inactive" };
                if(customer.CustomerImage!=null)
                {

                var res = DeleteImage(existingCust.CustomerImage);
                    if (res.Success == false) return new Response<string> { Message = "Customer image not exists", Success = false };
                var resp = await UploadImage(customer.CustomerImage);
                    if (res.Success == false) return new Response<string> { Message = "Customer image upload error", Success = false };
                 existingCust.CustomerImage = resp.Message;
                }
               

                        existingCust.FirstName = customer.FirstName;
                        existingCust.LastName = customer.LastName;
                        existingCust.MobileNumber = customer.MobileNumber;
                        existingCust.City = customer.City;
                        existingCust.Dob = customer.Dob;

                        await _dbContext.SaveChangesAsync();
                        return new Response<string> { Message = "Customer Updated", Success = true };
                   
              


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new Response<string> { Success = false, Message = "could not create account.." };
            }


        }
        public   Response<List<Customer>> GetCustomerByPagination(int pageSize, int pageNumber)
        {
            var res =  _dbContext.Customers.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            var data =  res.ToList();
            return new Response<List<Customer>>{ Data = data, Success = true,Message=_dbContext.Customers.ToList().Count.ToString() };
        }
        public  Response<List<Customer>> FilterCustomerByName(int pageSize, int pageNumber,string search)
        {
            var filterData= _dbContext.Customers.Where(c=>c.FirstName.Contains(search.ToLower())||c.LastName.Contains(search.ToLower())).ToList();
            var res = filterData.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return new Response<List<Customer>> { Data = res.ToList(), Success = true, Message = filterData.ToList().Count.ToString() };
        }

    }
}

