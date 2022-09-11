﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnumsNET;
using Microsoft.EntityFrameworkCore;
using OnsBudget.Data.Enums;
using OnsBudget.Data.Models;
using OnsBudget.Data.Shared;

namespace OnsBudget.Data.Services
{
    public class TransactionService
    {
        private readonly IDbContextFactory<OnsBudgetDbContext> dbFactory;

        public TransactionService( IDbContextFactory<OnsBudgetDbContext> dbFactory )
        {
            this.dbFactory = dbFactory;
        }

        public async Task<List<TransactionListModel>> GetAllUItgaven( )
        {
            await using var db = await dbFactory.CreateDbContextAsync( );

            var transactions = await db.Transactions.Where( x => x.BijAf == BijAf.Af ).Include(x => x.Category).OrderByDescending(x => x.Date).Select( x => new TransactionListModel( )
            {
                Id = x.Id,
                AccountNumber = x.AccountNumber,
                Amount = x.Amount,
                BijAf = x.BijAf,
                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name,
                Name = x.Name,
                Date = x.Date,
                Remark = x.Remark,
                TransactieTypeAsString = ( (TransactieType)x.TransactieType ).AsString( EnumFormat.Description )!,

        } ).ToListAsync();

            return transactions;
        }

        public async Task<List<TransactionListModel>> GetAllInkomsten( )
        {
            await using var db = await dbFactory.CreateDbContextAsync( );

            List<TransactionListModel> transactions = await db.Transactions.Where( x => x.BijAf == BijAf.Bij ).Include( x => x.Category ).Select( x => new TransactionListModel( )
            {
                Id = x.Id,
                AccountNumber = x.AccountNumber,
                Amount = x.Amount,
                BijAf = x.BijAf,
                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name,
                Name = x.Name,
                Date = x.Date,
                Remark = x.Remark,
                TransactieTypeAsString = ( (TransactieType)x.TransactieType ).AsString( EnumFormat.Description )!,
            } ).ToListAsync( );

            return transactions;
        }

        public async Task<List<TransactionListModel>> GetAllUnAssigned( )
        {
            await using var db = await dbFactory.CreateDbContextAsync( );

            List<TransactionListModel> transactions = await db.Transactions.Where( x => x.CategoryId == Constants.Categories.Unassigned ).OrderBy(x => x.Id).Include( x => x.Category ).Take(20).Select( x => new TransactionListModel( )
            {
                Id = x.Id,
                AccountNumber = x.AccountNumber,
                Amount = x.Amount,
                BijAf = x.BijAf,
                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name,
                Name = x.Name,
                Date = x.Date,
                Remark = x.Remark,
                TransactieTypeAsString = ( (TransactieType)x.TransactieType ).AsString( EnumFormat.Description )!,
            } ).ToListAsync( );

            return transactions;
        }

        public async Task SetCategory( TransactionListModel currentTransaction, CategoryTreeModel categoryTreeModel )
        {
            await using var db = await dbFactory.CreateDbContextAsync( );

            var transaction = await db.Transactions.AsTracking().SingleAsync( x => x.Id == currentTransaction.Id );
            transaction.CategoryId = categoryTreeModel.Id;

            await db.SaveChangesAsync( );
        }
    }
}
