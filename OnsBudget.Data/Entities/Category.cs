using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
#pragma warning disable CS8618

namespace OnsBudget.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public Category Parent { get; set; }
        public ICollection<Category> Children { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }

    public class CategoryMapping : IEntityTypeConfiguration<Category>
    {
        public void Configure( EntityTypeBuilder<Category> builder )
        {
            builder.HasKey( key => key.Id );
            builder.ToTable("Categories");
            builder.Property( x => x.Name ).IsRequired( true );
            builder.Property( x => x.ParentId ).IsRequired( false );
            builder.HasOne( x => x.Parent ).WithMany( x => x.Children ).HasForeignKey( x => x.ParentId );
        }
    }
}
