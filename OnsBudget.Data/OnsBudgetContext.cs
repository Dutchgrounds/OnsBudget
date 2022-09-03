using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnsBudget.Data.Entities;

namespace OnsBudget.Data
{
    public class OnsBudgetDbContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public OnsBudgetDbContext(DbContextOptions<OnsBudgetDbContext> options) : base(options)
        {}

        protected override void OnModelCreating( ModelBuilder builder )
        {
            base.OnModelCreating( builder );

            builder.ApplyConfigurationsFromAssembly( Assembly.GetExecutingAssembly( ) );
        }

        public DbSet<Transaction> Transactions { get; set; }
    }
}
