using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnsBudget.Data.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
    }

    public class CategoryMapping : IEntityTypeConfiguration<Transaction>
    {
        public void Configure( EntityTypeBuilder<Transaction> builder )
        {
            builder.HasKey( key => key.Id );
            builder.ToTable("Transactions");
            builder.Property( x => x.Date ).IsRequired( true );
        }
    }
}
