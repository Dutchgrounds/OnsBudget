using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnsBudget.Data.Models
{
    public class ImportResult
    {
        public bool Show { get; set; } = false;
        public int NewCounter { get; set; } = 0;
        public int DuplicateCounter { get; set; } = 0;
    }
}
