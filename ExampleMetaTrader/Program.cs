using System;
using System.Threading;
using System.Threading.Tasks;
using MtApi;
using ExampleMetaTrader.InformationDB;
using CT;

namespace MtApiConsole
{
    class Program
    {
        static readonly EventWaitHandle _connnectionWaiter = new AutoResetEvent(false);
        static readonly MtApiClient _mtapi = new MtApiClient();

        static void _mtapi_ConnectionStateChanged(object sender, MtConnectionEventArgs e)
        {
            switch (e.Status)
            {
                case MtConnectionState.Connecting:
                    Console.WriteLine("Connnecting...");
                    break;
                case MtConnectionState.Connected:
                    Console.WriteLine("Connnected.");
                    _connnectionWaiter.Set();
                    break;
                case MtConnectionState.Disconnected:
                    Console.WriteLine("Disconnected.");
                    _connnectionWaiter.Set();
                    break;
                case MtConnectionState.Failed:
                    Console.WriteLine("Connection failed.");
                    _connnectionWaiter.Set();
                    break;
            }
        }
        static DBMetaTrader CreateDBMetaTrader()
        {
            Ini programIni = Ini.ProgramIniFile;
            new MySql.Data.MySqlClient.MySqlConnection().Dispose();
            DBMetaTrader wrtier = new DBMetaTrader(programIni);
            return wrtier;
        }

        static void _mtapi_QuoteAdded(object sender, MtQuoteEventArgs e)
        {
            Console.WriteLine("Quote added with symbol {0}", e.Quote.Instrument);
            DBMetaTrader writer = CreateDBMetaTrader();
            writer.Insert(e.Quote.Ask,e.Quote.Bid,e.Quote.ExpertHandle,e.Quote.Instrument);
        }

        static void _mtapi_QuoteRemoved(object sender, MtQuoteEventArgs e)
        {
            Console.WriteLine("Quote removed with symbol {0}", e.Quote.Instrument);
        }

        static void _mtapi_QuoteUpdate(object sender, MtQuoteEventArgs e)
        {
            Console.WriteLine("Quote updated: {0} - {1} : {2}", e.Quote.Instrument, e.Quote.Bid, e.Quote.Ask);
            DBMetaTrader writer = CreateDBMetaTrader();
            writer.Insert(e.Quote.Ask, e.Quote.Bid, e.Quote.ExpertHandle, e.Quote.Instrument);
        }

        static void Main(string[] args)
        {
            CreateDBMetaTrader();
            Console.WriteLine("Application started.");

            _mtapi.ConnectionStateChanged += _mtapi_ConnectionStateChanged;
            _mtapi.QuoteAdded += _mtapi_QuoteAdded;
            _mtapi.QuoteRemoved += _mtapi_QuoteRemoved;
            _mtapi.QuoteUpdate += _mtapi_QuoteUpdate;

            _mtapi.BeginConnect(8222);
            _connnectionWaiter.WaitOne();

            if (_mtapi.ConnectionState == MtConnectionState.Connected)
            {
                Run();
            }

            Console.WriteLine("Application finished. Press any key...");
            Console.ReadKey();
        }

        private static void Run()
        {
            ConsoleKeyInfo cki;
            do
            {
                cki = Console.ReadKey();
                //switch (cki.KeyChar.ToString())
                //{
                //    case "b":
                //        Buy();
                //        break;
                //    case "s":
                //        Sell();
                //        break;
                //}
            } while (cki.Key != ConsoleKey.Escape);

            _mtapi.BeginDisconnect();
            _connnectionWaiter.WaitOne();
        }

        //private static async void Buy()
        //{
        //    const string symbol = "EURUSD";
        //    const double volume = 0.1;
        //    MqlTradeResult tradeResult = null;
        //    var retVal = await Execute(() => _mtapi.Buy(out tradeResult, volume, symbol));
        //    Console.WriteLine($"Buy: symbol EURUSD retVal = {retVal}, result = {tradeResult}");
        //}

        //private static async void Sell()
        //{
        //    const string symbol = "EURUSD";
        //    const double volume = 0.1;
        //    MqlTradeResult tradeResult = null;
        //    var retVal = await Execute(() => _mtapi.Sell(out tradeResult, volume, symbol));
        //    Console.WriteLine($"Sell: symbol EURUSD retVal = {retVal}, result = {tradeResult}");
        //}

        //private static async Task<TResult> Execute<TResult>(Func<TResult> func)
        //{
        //    return await Task.Factory.StartNew(() =>
        //    {
        //        var result = default(TResult);
        //        try
        //        {
        //            result = func();
        //        }
        //        catch (ExecutionException ex)
        //        {
        //            Console.WriteLine($"Exception: {ex.ErrorCode} - {ex.Message}");
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Exception: {ex.Message}");
        //        }

        //        return result;
        //    });
        //}
    }
}