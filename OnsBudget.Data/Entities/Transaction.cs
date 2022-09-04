using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnsBudget.Data.Enums;

namespace OnsBudget.Data.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string? CounterAccountNumber { get; set; }
        public TransactieType TransactieType { get; set; }
        public BijAf BijAf { get; set; }
        public string Remark { get; set; } = string.Empty;
        public Decimal Amount { get; set; }

        public override string ToString()
        {
            var result = $"{Date.ToString("dd-MM-yyyy")} - {Name} - {AccountNumber} - {CounterAccountNumber} - {TransactieType.ToString()} - {BijAf.ToString()} - {Amount} - {Remark}";

            return result;
        }
    }

    public class TransactionMapping : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(key => key.Id);
            builder.ToTable("Transactions");
            builder.Property(x => x.Date).IsRequired(true);
            builder.Property(x => x.Name).IsRequired(true);
            builder.Property(x => x.AccountNumber).IsRequired(true);
            builder.Property(x => x.CounterAccountNumber).IsRequired(false);
            builder.Property(x => x.TransactieType).IsRequired(true);
            builder.Property(x => x.BijAf).IsRequired(true);
            builder.Property(x => x.Amount).IsRequired(true).HasPrecision(18, 2);
            builder.Property(x => x.Remark).IsRequired(false);
        }
    }
}
