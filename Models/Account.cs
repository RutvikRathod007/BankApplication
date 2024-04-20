using BankApplication.Models.Dtos.AccoutDtos;
using System;
using System.Collections.Generic;

namespace BankApplication.Models
{
    public partial class Account:AccountDto
    {
        public Account()
        {
            TransactionTbls = new HashSet<TransactionTbl>();
        }

       
       

        public virtual Customer Customer { get; set; } = null!;
        public virtual ICollection<TransactionTbl> TransactionTbls { get; set; }
    }
}
