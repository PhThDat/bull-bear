using System.IO;
using System;
class LogWriter
{
    static string filePath = "C:\\Users\\Dat\\Desktop\\abcde\\Log.csv";

    public static void WriteDescription()
    {
        File.WriteAllText(filePath, "Date, High, Low, Close, MA20, BOLU, BOLD");
    }
    public static void AppendCandleData(CandleStick candle)
    {
        string content = "\n" + candle.date.ToString() + "," + candle.highPrice.ToString() + "," + candle.lowPrice.ToString() + "," 
        + candle.closePrice.ToString() + "," + candle.MA20.ToString() + "," + candle.BOLU.ToString() + "," + candle.BOLD.ToString();
        File.AppendAllText(filePath, content);
    }
}