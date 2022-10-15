using System;
using System.Linq;
class BinanceAccount
{
    public double balance = 40, FTMBalance = 0, lastBuyInFTM = 0, lastSellInFTM = 0, stopLoss;

    public void BuyFTMCoin(double coinAmount, CandleStickList candles)
    {
        double price = coinAmount * candles.list.First().closePrice;
        if (BuyConditionMet(coinAmount, candles))
        {
            lastBuyInFTM = candles.list.First().closePrice;
            balance -= price - price/1000;
            FTMBalance += coinAmount;
            stopLoss = candles.list.First().closePrice * 999/1000;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(coinAmount + " FTM coins are bought at price " + candles.list.First().closePrice);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    public void SellFTMCoin(double coinAmount, CandleStickList candles)
    {
        double price = coinAmount * candles.list.First().closePrice;
        if (SellConditionMet(coinAmount, candles))
        {
            lastSellInFTM = candles.list.First().closePrice;
            balance += price - price/1000;
            FTMBalance -= coinAmount;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(coinAmount + " FTM coins are sold at price " + candles.list.First().closePrice);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    bool BuyConditionMet(double coinAmount, CandleStickList candles)
    {
        double price = coinAmount * candles.list.First().closePrice;
        if (FTMBalance == 0 && candles.stochRSI14GoBeyond20)
            if (price <= balance && coinAmount >= 0)
                return true;
        return false;
    }

    bool SellConditionMet(double coinAmount, CandleStickList candles)
    {   
        double price = coinAmount * candles.list.First().closePrice;
        if (candles.stochRSI14GoBelow80 || candles.list.First().closePrice <= stopLoss)
            if (coinAmount >= 0 && coinAmount <= FTMBalance && FTMBalance > 0)
                return true;
        return false;
    }

    public void CalcStopLoss(CandleStickList candles)
    {
        if (FTMBalance > 0)
        {
            double newStopLoss = candles.list.First().closePrice - (candles.list.First().BOLU - candles.list.First().BOLD).Value / 5;
            if (stopLoss < newStopLoss)
                stopLoss = newStopLoss;
        }
    }
}                             