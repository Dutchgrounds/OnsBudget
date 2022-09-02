using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace OnsBudget.Data.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public List<AppRole> Roles { get; set; } = null!;
    }
}
