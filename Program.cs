using System;
using System.Timers;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace abcde
{
    class Program
    {
        static BinanceAccount account = new BinanceAccount();
        static CandleStickList candles = new CandleStickList();
        static async Task CrawlData(string url)
        {
            var res = await APICaller.GetMsg(url);
            var result = JsonConvert.DeserializeObject<dynamic[][]>(res);
            for (long i = result.Length - 1; i >= 0; i--)
                candles.list.Add(new CandleStick(result[i]));
        }
        static async void SetUpData()
        {
            await CrawlData("https://api.binance.com/api/v3/klines?symbol=FTMBUSD&interval=1m&limit=1000");
            candles.CalcCurrentRSI(periodNum: 14);
            candles.CalcCurrentRSI(periodNum: 6);
            //candles.CalcCurrentMACD();
            candles.ShortenList(candleAmountToKeep: 50);
            candles.CalcMA(dayNum: 6);
            candles.CalcMA(dayNum: 20);
            candles.CalcCurrentBOLL(day: 20, k: 2);
            candles.CalcCurrentStochRSI14(smoothenArg: 3);

            LogWriter.WriteDescription();
            foreach (CandleStick candle in candles.list)
                LogWriter.AppendCandleData(candle);
            Console.WriteLine("Pre-load data completed");
        }
        static async void OnTimerTrigger()
        {
            var res = await APICaller.GetMsg("https://api.binance.com/api/v3/klines?symbol=FTMBUSD&interval=1m&limit=2");
            var result = JsonConvert.DeserializeObject<dynamic[][]>(res);
            if ((new CandleStick(result[1])).date.Hour == candles.list.First().date.Hour && (new CandleStick(result[1])).date.Minute == candles.list.First().date.Minute)
                candles.list.RemoveAt(0);
            candles.list.RemoveAt(0);
            candles.list.Insert(0, new CandleStick(result[0]));
            candles.list.Insert(0, new CandleStick(result[1]));

            candles.ShortenList(40);
            candles.UpdateNewCandleData();
            account.CalcStopLoss(candles);

            account.SellFTMCoin(account.FTMBalance, candles);
            account.BuyFTMCoin((int)(account.balance / candles.list.First().closePrice), candles);

            if (candles.list.First().closePrice * 997/1000 > account.stopLoss && account.FTMBalance > 0)
                account.stopLoss = candles.list.First().closePrice * 997/1000;

            Console.WriteLine(candles.list.First().date + ", RSI14: " + candles.list.First().RSI14 + ", Stoch RSI14: " + candles.list.First().smoothenK);
            Console.WriteLine("Total balance in BUSD: " + (account.balance + account.FTMBalance * candles.list.First().closePrice));
            Console.WriteLine("BUSD balance: " + account.balance);
            Console.WriteLine("Stop loss: " + account.stopLoss);
            Console.WriteLine("FTM coins: " + account.FTMBalance + "\n");

            // LogWriter.WriteDescription();
            // foreach (CandleStick candle in candles.list)
            //     LogWriter.AppendCandleData(candle);
        }

        static System.Timers.Timer t;
        static void Main(string[] args)
        {
            SetUpData();
            t = new System.Timers.Timer();
            t.AutoReset = true;
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);

            Timer tempTimer = new Timer();
            tempTimer.AutoReset = false;
            tempTimer.Interval = (61 - DateTime.Now.Second) * 1000 - DateTime.Now.Millisecond;
            tempTimer.Elapsed += new ElapsedEventHandler(t_Elapsed);
            tempTimer.Start();
            Console.ReadLine();
        }
        static void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnTimerTrigger();
            t.Interval = 15000;
            t.Start();
        }
    }
}
