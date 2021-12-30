using CT;
using CT.Data;
using ExampleMetaTrader.InformationTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleMetaTrader.InformationDB
{
    class DBMetaTrader
    {
        private IDatabase database;
        private ITable<Price> tPrice;
        public DBMetaTrader(Ini programIni)
        {
            string connectionString = programIni.ReadString("Settings", "DBConnection");
            database = Connector.ConnectDatabase(connectionString, DbConnectionOptions.AllowCreate);
            tPrice = database.GetTable<Price>(TableFlags.AllowCreate);
        }
        
        public void Insert(double ask, double bid, int expertHandle, string instrument)
        {
            Price insert = new Price()
            {
                Ask = ask,
                Bid = bid,
                ExpertHandle = expertHandle,
                Instrument = instrument,
                DateUpdate = DateTime.Now
            };
            tPrice.Insert(insert);
        }
        public void Update(long id, double ask, double bid, int expertHandle, string instrument)
        {
            Price update = new Price()
            {
                Id = id,
                Ask = ask,
                Bid = bid,
                ExpertHandle = expertHandle,
                Instrument = instrument,
                DateUpdate = DateTime.Now
        };
            tPrice.Update(update);
        }
        public void Delete(long id)
        {
            tPrice.Delete(id);
        }
    }
}
