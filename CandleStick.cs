using System;

class CandleStick
{
    DateTime firstDate = new DateTime(1970, 1, 1, 7, 0, 0);
    public double openPrice, highPrice, lowPrice, closePrice, volume;
    public double? RSI6 = null, RSI14 = null, rsiMaxEma6 = 0, rsiABSEma6 = 0, rsiABSEma14 = 0, rsiMaxEma14 = 0;
    //public double? MACD = null, DIF, DEA = 0, EMA12 = null, EMA26 = null;
    public double? MA6 = null, MA20 = null, BOLU = null, BOLD = null;
    public double? K = null, smoothenK = null;
    public DateTime date, closeDate;

    public CandleStick(dynamic[] msg)
    {
        date = firstDate.AddMilliseconds(msg[0]);
        openPrice = Double.Parse(msg[1]);
        highPrice = Double.Parse(msg[2]);
        lowPrice = Double.Parse(msg[3]);
        closePrice = Double.Parse(msg[4]);
        volume = Double.Parse(msg[5]);
        closeDate = firstDate.AddMilliseconds(msg[6]);
    }
}