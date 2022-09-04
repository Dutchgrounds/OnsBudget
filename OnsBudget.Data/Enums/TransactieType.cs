using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnsBudget.Data.Enums
{
    public enum TransactieType
    {
        [Description( "iDEAL" )]
        ID,
        [Description( "Betaalautomaat" )]
        BA,
        [Description( "Incasso" )]
        IC,
        [Description( "Overschrijving" )]
        OV,
        [Description( "Online bankieren" )]
        GT,
        [Description( "Diversen" )]
        DV,
        [Description( "Verzamelbetaling" )]
        VZ,
        [Description( "Geldautomaat" )]
        GM
    }
}
