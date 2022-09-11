using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnsBudget.Data.Entities;
using OnsBudget.Data.Enums;

namespace OnsBudget.Data.Models
{
    public class TransactionListModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string TransactieTypeAsString { get; set; } = string.Empty;
        public BijAf BijAf { get; set; }
        public string Remark { get; set; } = string.Empty;
        public Decimal Amount { get; set; }
        
        public string CategoryName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
    }
}
