using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnsBudget.Data.Entities;

namespace OnsBudget.Data
{
    public class OnsBudgetDbContext : IdentityDbContext<AppUser, AppRole, int>
    {

    }
}
