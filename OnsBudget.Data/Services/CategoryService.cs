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

            var nodes = await db.Categories.Where(x => x.Id > 1).Select( x => new CategoryTreeModel( ) { Id = x.Id, Name = x.Name, ParentId = x.ParentId, Children = new List<CategoryTreeModel>( ) } ).ToListAsync( );

            foreach( var node in nodes )
            {
                node.Children = nodes.Where( x => x.ParentId == node.Id ).ToList( );
            }

            var result = nodes.Where( x => x.ParentId.HasValue == false ).ToList( );

            return result;
        }
    }
}
