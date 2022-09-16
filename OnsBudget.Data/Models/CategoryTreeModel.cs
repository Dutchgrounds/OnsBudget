using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnsBudget.Data.Models
{
    public class CategoryTreeModel
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public int Depth { get; set; }
        public List<CategoryTreeModel> Children { get; set; } = new List<CategoryTreeModel>( );
    }
}