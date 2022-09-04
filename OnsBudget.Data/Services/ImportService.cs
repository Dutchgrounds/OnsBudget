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

namespace OnsBudget.Data.Services
{
    public class ImportService
    {
        private readonly IDbContextFactory<OnsBudgetDbContext> dbFactory;

        public ImportService(IDbContextFactory<OnsBudgetDbContext> dbFactory )
        {
            this.dbFactory = dbFactory;
        }

        public async Task ImportFile( StreamReader streamReader )
        {
            await using var db = await dbFactory.CreateDbContextAsync( );

            string line;
            var index = 0;
            while ( ( line = streamReader.ReadLine( ) ) != null )
            {
                if(index > 0)
                {
                    var transaction = await ImportLine( line );
                    await db.Transactions.AddAsync( transaction );
                }
                    
                index++;
            }
        }

        public async Task<Transaction> ImportLine( string line )
        {
            var parts = line.Split( ";" );

            var transaction = new Transaction( );

            transaction.Date = GetDate(parts[0].Substring(1,8) );
            transaction.Name = StripQuotes( parts[1] );
            transaction.AccountNumber = StripQuotes( parts[ 2 ] );
            
            var counterAccountNumber = parts[ 3 ];
            if( counterAccountNumber != "\"\"" )
            {
                transaction.CounterAccountNumber = StripQuotes( parts[ 3 ] );
            }

            var code = StripQuotes( parts[ 4 ] );
            transaction.TransactieType = (TransactieType)Enum.Parse( typeof(TransactieType ), code );

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
