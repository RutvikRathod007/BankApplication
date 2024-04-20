using BankApplication.Models.Dtos.TransactionDtos;
using System;
using System.Collections.Generic;

namespace BankApplication.Models
{
    public partial class TransactionTbl:DisplayTransactionsDto
    {
       
        public virtual Account AccNumberNavigation { get; set; } = null!;
    }
}
