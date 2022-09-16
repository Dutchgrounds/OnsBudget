using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnsBudget.Data.Entities;
using OnsBudget.Data.Enums;
using OnsBudget.Data.Models;
using OnsBudget.Data.Shared;

namespace OnsBudget.Data.Services
{
    public class ImportService
    {
        private readonly IDbContextFactory<OnsBudgetDbContext> dbFactory;

        public ImportService(IDbContextFactory<OnsBudgetDbContext> dbFactory )
        {
            this.dbFactory = dbFactory;
        }

        public async Task<ImportResult> ImportFile( StreamReader streamReader )
        {
            await using var db = await dbFactory.CreateDbContextAsync( );

            string line;
            var index = 0;
            var duplicateCounter = 0;
            var newCounter = 0;
            while ( ( line = streamReader.ReadLine( )! ) != null )
            {
                if(index > 0)
                {
                    var transaction = ImportLine( line );
                    

                    //check if not duplicate!
                    var duplicates = await db.Transactions.AnyAsync( x => x.Amount == transaction.Amount && x.Name == transaction.Name && x.Date == transaction.Date );

                    if( !duplicates )
                    {
                        transaction.CategoryId = 1;
                        transaction.Category = db.Categories.AsTracking().Single( x => x.Id == Constants.Categories.Unassigned );
                        await db.Transactions.AddAsync( transaction );
                        await db.SaveChangesAsync( );

                        newCounter++;
                    }
                    else
                    {
                        duplicateCounter++;
                    }
                    
                }
                    
                index++;
            }

            return new ImportResult { Show = true, NewCounter = newCounter, DuplicateCounter = duplicateCounter };
        }

        public Transaction ImportLine( string line )
        {
            var parts = line.Split( ";" );

            var transaction = new Transaction
            {
                CategoryId = 1,
                Date = GetDate(parts[0].Substring(1,8) ),
                Name = StripQuotes( parts[1] ),
                AccountNumber = StripQuotes( parts[ 2 ] )
            };

            var counterAccountNumber = parts[ 3 ];
            if( counterAccountNumber != "\"\"" )
            {
                transaction.CounterAccountNumber = StripQuotes( parts[ 3 ] );
            }

            var code = StripQuotes( parts[ 4 ] );
            transaction.TransactieType = (TransactieType)Enum.Parse( typeof(TransactieType ), code );

            var bijaf = StripQuotes( parts[ 5 ] );
            transaction.BijAf = (BijAf)Enum.Parse( typeof(BijAf ), bijaf );

            Decimal bedrag = 0;
            decimal.TryParse(StripQuotes( parts[ 6 ] ), NumberStyles.AllowDecimalPoint, new CultureInfo( "nl-NL" ), out bedrag );

            transaction.Amount = bedrag;
            transaction.Remark = StripQuotes( parts[ 8 ] );
            
            return transaction;
        }

        private DateTime GetDate( string text )
        {
            var year = text.Substring( 0, 4 );
            var month = text.Substring( 4, 2 );
            var day = text.Substring( 6, 2 );
            
            var result = new DateTime(int.Parse(year), int.Parse(month ), int.Parse(day ));

            return result;
        }

        private string StripQuotes( string text )
        {
            return text.Substring( 1, text.Length - 2 );
        }
    }
}
