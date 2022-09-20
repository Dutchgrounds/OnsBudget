using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnsBudget.Data.Entities;
using OnsBudget.Data.Models;
using OnsBudget.Data.Shared;

namespace OnsBudget.Data.Services
{
    public class CategoryService
    {
        private readonly IDbContextFactory<OnsBudgetDbContext> dbFactory;
        private readonly ILogger<CategoryService> logger;

        public CategoryService(IDbContextFactory<OnsBudgetDbContext> dbFactory, ILogger<CategoryService> logger )
        {
            this.dbFactory = dbFactory;
            this.logger = logger;
        }

        public async Task<List<CategoryTreeModel>> GetCategoryTree( )
        {
            await using var db = await dbFactory.CreateDbContextAsync( );

            var nodes = await db.Categories
                                .Where(x => x.Id > Constants.Categories.Unassigned)
                                .OrderBy( x => x.SortOrder )
                                .Select( x => new CategoryTreeModel( )
                                {
                                    Id = x.Id, 
                                    Name = x.Name, 
                                    ParentId = x.ParentId, 
                                    SortOrder = x.SortOrder
                                } ).ToListAsync( );

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
                                .Where( x => x.Id > Constants.Categories.Unassigned )
                                .OrderBy(x => x.SortOrder)
                                .Select( x => new BudgetCategoryTreeModel( )
                                {
                                    Id = x.Id, 
                                    Name = x.Name, 
                                    ParentId = x.ParentId, 
                                    SortOrder = x.SortOrder
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
                    node.Amount = groupedAmounts.Single( x => x.Key == node.Id ).Sum;
                }
                
                node.Children = nodes.Where( x => x.ParentId == node.Id ).OrderBy( x => x.SortOrder ).ToList( );
            }

            foreach( var node in nodes.Where(x => x.ParentId == null) )
            {
                CalculateAmoutOfAllChildren(node);
            }

            var totalAmount = nodes.Sum( x => x.Amount );
            foreach( var node in nodes )
            {
                node.Percentage = (node.AmountTotal / totalAmount) * 100;
            }

            var result = nodes.Where( x => x.ParentId.HasValue == false ).ToList( );

            return result;
        }

        private void CalculateAmoutOfAllChildren( BudgetCategoryTreeModel node )
        {
            if( node.Children.Any( ) )
            {
                foreach( var child in node.Children )
                {
                    child.Depth = node.Depth + 1;
                    CalculateAmoutOfAllChildren( child );
                }
            }

            node.AmountChildren = node.Children.Sum( x => x.AmountTotal );
        }

        public async Task AddRootItem( )
        {
            await using var db = await dbFactory.CreateDbContextAsync( );

            var maxSortOrder = db.Categories.Where( x => x.ParentId == null ).Max( x => x.SortOrder ) + 1;
            var rootItem = new Category( ) { Name = "Nieuwe categorie", SortOrder = maxSortOrder };
            db.Categories.Add( rootItem );

            await db.SaveChangesAsync();
        }

        public async Task AddChild( int parentId )
        {
            await using var db = await dbFactory.CreateDbContextAsync( );

            var lastEntry = db.Categories.OrderBy( x => x.SortOrder ).LastOrDefault( );
            var maxSortOrder = lastEntry != null ? lastEntry.SortOrder + 1 : 1;

            var rootItem = new Category( ) { Name = $"Nieuwe categorie {DateTime.Now.ToShortTimeString()}", SortOrder = maxSortOrder, ParentId = parentId};
            db.Categories.Add( rootItem );

            await db.SaveChangesAsync( );
        }

        public async Task<bool> CanDelete( int categoryId )
        {
            await using var db = await dbFactory.CreateDbContextAsync( );
            
            var inuse = await db.Transactions.AnyAsync( x => x.CategoryId == categoryId);
            var hasChildren = await db.Categories.AnyAsync( x => x.ParentId == categoryId );
            
            return (inuse || hasChildren) && false;
        }
        
        public async Task Delete( int nodeId )
        {
            await using var db = await dbFactory.CreateDbContextAsync( );

            var category = await db.Categories.SingleOrDefaultAsync( x => x.Id == nodeId );
            if( category != null )
            {
                db.Categories.Remove( category );
                await db.SaveChangesAsync( );
            }
        }

        public async Task SaveCategory( NodeEditModel nodeEditModel )
        {
            await using var db = await dbFactory.CreateDbContextAsync( );
            
            var category = await db.Categories.AsTracking().SingleOrDefaultAsync( x => x.Id == nodeEditModel.Id );
            if( category != null )
            {
                category.Name = nodeEditModel.Name;
                //category.ParentId = nodeEditModel.ParentId;
                
                await db.SaveChangesAsync( );
            }
        }
    }
}
