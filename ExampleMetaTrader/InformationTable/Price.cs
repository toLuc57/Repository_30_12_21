using CT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleMetaTrader.InformationTable
{
    [Table("Price")]
    public struct Price
    {
        [Field (Flags = FieldFlags.ID | FieldFlags.AutoIncrement)]
        public long Id;
        [Field]
        public double Ask ;
        [Field]
        public double Bid ;
        [Field]
        public int ExpertHandle ;
        [Field]
        public string Instrument ;
        [Field]
        public DateTime DateUpdate;
    }
}
