using System.ComponentModel.DataAnnotations;

namespace OnsBudget.Data.Models
{
    public class NodeEditModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
