namespace OnsBudget.Data.Models
{
    public class BudgetCategoryTreeModel
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public int Depth { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountChildren { get; set; }
        public decimal AmountTotal => Amount + AmountChildren;
        public decimal Percentage { get; set; }

        public List<BudgetCategoryTreeModel> Children { get; set; } = new List<BudgetCategoryTreeModel>( );
    }
}

