using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnsBudget.Data.Models;

namespace OnsBudget.Data.Services
{
    public class CategoryService
    {
        private readonly IDbContextFactory<OnsBudgetDbContext> dbFactory;

        public CategoryService(IDbContextFactory<OnsBudgetDbContext> dbFactory )
        {
            this.dbFactory = dbFactory;
        }

        public async Task<List<CategoryTreeModel>> GetCategoryTree( )
        {
            await using var db = await dbFactory.CreateDbContextAsync( );

            var nodes = await db.Categories.Where(x => x.Id > 1).Select( x => new CategoryTreeModel( ) { Id = x.Id, Name = x.Name, ParentId = x.ParentId, SortOrder = x.SortOrder, Children = new List<CategoryTreeModel>( ) } ).ToListAsync( );

            foreach( var node in nodes )
            {
                node.Children = nodes.Where( x => x.ParentId == node.Id ).OrderBy(x => x.SortOrder).ToList( );
            }

            var result = nodes.Where( x => x.ParentId.HasValue == false ).ToList( );

            return result;
        }

        public async Task<List<BudgetCategoryTreeModel>> GetBudgetCategoryTree( )
        {
            await using var db = await dbFactory.CreateDbContextAsync( );

            var nodes = await db.Categories
                                .Where( x => x.Id > 1 )
                                .Select( x => new BudgetCategoryTreeModel( )
                                {
                                    Id = x.Id, 
                                    Name = x.Name, 
                                    ParentId = x.ParentId, 
                                    SortOrder = x.SortOrder, 
                                    Amount = 0,
                                    AmountChildren = 0,
                                    Children = new List<BudgetCategoryTreeModel>( )
                                } ).ToListAsync( );

            var groupedAmounts = db.Transactions
                                   .Where(x => x.CategoryId > 1)
                .GroupBy( x => x.CategoryId )
                .Select( x => new
                {
                    x.Key,
                    Sum = x.Sum( c => c.Amount )
                } ).ToList();

            foreach( var node in nodes )
            {
                if( groupedAmounts.Any( x => x.Key == node.Id ) )
                {
                    node.Amount = groupedAmounts.SingleOrDefault( x => x.Key == node.Id ).Sum;
                }
                
                node.Children = nodes.Where( x => x.ParentId == node.Id ).OrderBy( x => x.SortOrder ).ToList( );
            }

            foreach( var node in nodes.Where(x => x.ParentId == null) )
            {
                CalculateChildAmounts(node);
            }

            var totalAmount = nodes.Sum( x => x.Amount );
            foreach( var node in nodes )
            {
                node.Percentage = (node.AmountTotal / totalAmount) * 100;
            }

            var result = nodes.Where( x => x.ParentId.HasValue == false ).ToList( );

            return result;
        }

        private void CalculateChildAmounts( BudgetCategoryTreeModel node )
        {
            if( node.Children.Any( ) )
            {
                foreach( var child in node.Children )
                {
                    child.Depth = node.Depth + 1;
                    CalculateChildAmounts( child );
                }
            }

            node.AmountChildren = node.Children.Sum( x => x.AmountTotal );
        }
        
    }
}
