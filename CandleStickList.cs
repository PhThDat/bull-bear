using System;
using System.Linq;
using System.Collections.Generic;

class CandleStickList
{
    public List<CandleStick> list = new List<CandleStick>();
    public bool stochRSI14GoBeyond20, stochRSI14GoBelow80;
    // public void ShortenList(int candleAmountToKeep)
    // {
    //     List<CandleStick> newList = new List<CandleStick>();
    //     if (candleAmountToKeep <= 1)
    //         return;

    //     for (int i = 0; i < candleAmountToKeep; i++)
    //         newList.Add(list.ElementAt(i));

    //     list = newList;
    // }

    public void ShortenList(int candleAmountToKeep)
    {
        while (list.Count > candleAmountToKeep)
            list.RemoveAt(list.Count - 1);
    }

    public void UpdateNewCandleData()
    {
        UpdateMA(6);
        UpdateMA(20);
        //UpdateMACD();
        UpdateRSI(6);
        UpdateRSI(14);
        UpdateStochRSI14(3);
        UpdateBOLL(20, 2);
    }

    double GetFirstEMA(int dayNum)
    {
        if (list.Count <= dayNum)
            return 0;

        double sum = 0;
        for (int i = list.Count - 1; i >= list.Count - dayNum; i--)
            sum += list.ElementAt(i).closePrice;

        return sum / dayNum;
    }

    // public void CalcCurrentMACD()
    // {
    //     if (list.Count <= 26)
    //         return;

    //     list.ElementAt(list.Count - 12).EMA12 = GetFirstEMA(12);
    //     for (int i = list.Count - 13; i >= list.Count - 26; i--)
    //         list.ElementAt(i).EMA12 = list.ElementAt(i + 1).EMA12 * 11/13 + list.ElementAt(i).closePrice * 2/13;

    //     list.ElementAt(list.Count - 26).EMA26 = GetFirstEMA(26);
    //     for (int i = list.Count - 27; i >= 0; i--)
    //     {
    //         list.ElementAt(i).EMA12 = list.ElementAt(i + 1).EMA12 * 11/13 + list.ElementAt(i).closePrice * 2/13;;
    //         list.ElementAt(i).EMA26 = list.ElementAt(i + 1).EMA26 * 25/27 + list.ElementAt(i).closePrice * 2/27;;
    //         list.ElementAt(i).DEA = list.ElementAt(i).DEA * 8/10 + (list.ElementAt(i).EMA12 - list.ElementAt(i).EMA26) * 2/10;
    //         list.ElementAt(i).DIF = list.ElementAt(i).EMA12 - list.ElementAt(i).EMA26;
    //         list.ElementAt(i).MACD = list.ElementAt(i).DIF - list.ElementAt(i).DEA;
    //     }
    // }

    // public void UpdateMACD()
    // {
    //     for (int i = 1; i >= 0; i--)
    //     {
    //         list.ElementAt(i).EMA12 = list.ElementAt(i + 1).EMA12 * 11/13 + list.ElementAt(i).closePrice * 2/13;
    //         list.ElementAt(i).EMA26 = list.ElementAt(i).EMA26 * 25/27 + list.ElementAt(i).closePrice * 2/27;
    //         list.ElementAt(i).DIF = list.ElementAt(i).EMA12 - list.ElementAt(i).EMA26;
    //         list.ElementAt(i).DEA = list.ElementAt(i).DEA * 8/10 + list.ElementAt(i).DIF * 2/10;
    //     }
    // }

    public void CalcCurrentRSI(int periodNum)
    {
        if (list.Count < 6)
            return;

        double rMax, rAbs;
        for (int i = list.Count - 2; i > list.Count - periodNum - 1; i--)
        {
            double r = list.ElementAt(i).closePrice - list.ElementAt(i + 1).closePrice;
            if (r < 0)
            {
                rMax = 0;
                rAbs = -r;
            }else rMax = rAbs = r;
            if (periodNum == 6)
            {
                list.ElementAt(i).rsiMaxEma6 = (rMax + (periodNum - 1) * list.ElementAt(i + 1).rsiMaxEma6) / periodNum;
                list.ElementAt(i).rsiABSEma6 = (rAbs + (periodNum - 1) * list.ElementAt(i + 1).rsiABSEma6) / periodNum;
            }else if (periodNum == 14)
            {
                list.ElementAt(i).rsiMaxEma14 = (rMax + (periodNum - 1) * list.ElementAt(i + 1).rsiMaxEma14) / periodNum;
                list.ElementAt(i).rsiABSEma14 = (rAbs + (periodNum - 1) * list.ElementAt(i + 1).rsiABSEma14) / periodNum;
            }
        }

        for (int i = list.Count - periodNum - 1; i >= 0; i--)
        {
            double r = list.ElementAt(i).closePrice - list.ElementAt(i + 1).closePrice;
            if (r < 0)
            {
                rMax = 0;
                rAbs = -r;
            }else rMax = rAbs = r;
            if (periodNum == 6)
            {
                list.ElementAt(i).rsiMaxEma6 = (rMax + (periodNum - 1) * list.ElementAt(i + 1).rsiMaxEma6) / periodNum;
                list.ElementAt(i).rsiABSEma6 = (rAbs + (periodNum - 1) * list.ElementAt(i + 1).rsiABSEma6) / periodNum;
            }else if (periodNum == 14)
            {
                list.ElementAt(i).rsiMaxEma14 = (rMax + (periodNum - 1) * list.ElementAt(i + 1).rsiMaxEma14) / periodNum;
                list.ElementAt(i).rsiABSEma14 = (rAbs + (periodNum - 1) * list.ElementAt(i + 1).rsiABSEma14) / periodNum;
            }

            if (periodNum == 6)
                list.ElementAt(i).RSI6 = list.ElementAt(i).rsiMaxEma6 / list.ElementAt(i).rsiABSEma6 * 100;
            if (periodNum == 14)
                list.ElementAt(i).RSI14 = list.ElementAt(i).rsiMaxEma14 / list.ElementAt(i).rsiABSEma14 * 100;
        }
    }

    public void UpdateRSI(int periodNum)
    {
        if (list.Count <= 1)
            return;

        double rMax, rAbs;
        for (int i = 1; i >= 0; i--)
        {
            double r = list.ElementAt(i).closePrice - list.ElementAt(i + 1).closePrice;
            if (r < 0)
            {
                rMax = 0;
                rAbs = -r;
            }else rMax = rAbs = r;
            if (periodNum == 6)
            {
                list.ElementAt(i).rsiMaxEma6 = (rMax + (periodNum - 1) * list.ElementAt(i + 1).rsiMaxEma6) / periodNum;
                list.ElementAt(i).rsiABSEma6 = (rAbs + (periodNum - 1) * list.ElementAt(i + 1).rsiABSEma6) / periodNum;
            }else if (periodNum == 14)
            {
                list.ElementAt(i).rsiMaxEma14 = (rMax + (periodNum - 1) * list.ElementAt(i + 1).rsiMaxEma14) / periodNum;
                list.ElementAt(i).rsiABSEma14 = (rAbs + (periodNum - 1) * list.ElementAt(i + 1).rsiABSEma14) / periodNum;
            }

            if (periodNum == 6)
                list.ElementAt(i).RSI6 = list.ElementAt(i).rsiMaxEma6 / list.ElementAt(i).rsiABSEma6 * 100;
            if (periodNum == 14)
                list.ElementAt(i).RSI14 = list.ElementAt(i).rsiMaxEma14 / list.ElementAt(i).rsiABSEma14 * 100;
        }
    }

    public void CalcMA(int dayNum)
    {
        if (dayNum > list.Count)
            return;
        
        for (int i = 0; i < list.Count - dayNum; i++)
        {
            double sum = 0;
            for (int j = i; j < i + dayNum; j++)
                sum += list.ElementAt(j).closePrice;

            if (dayNum == 6) list.ElementAt(i).MA6 = sum / dayNum;
            else if (dayNum == 20) list.ElementAt(i).MA20 = sum / dayNum;
        }
    }

    public void UpdateMA(int dayNum)
    {
        if (dayNum > list.Count)
            return;
        
        for (int i = 1; i >= 0; i--)
        {
            double sum = 0;
            for (int j = i; j < i + dayNum; j++)
                sum += list.ElementAt(j).closePrice;
            
            if (dayNum == 6)
                list.ElementAt(i).MA6 = sum / dayNum;
            else if (dayNum == 20)
                list.ElementAt(i).MA20 = sum / dayNum;
        }
    }

    public void CalcCurrentBOLL(int day, int k)
    {
        if (list.Count < day)
            return;

        for (int i = list.Count - day; i >= 0; i--)
        {
            if (list.ElementAt(i).MA20 == null)
                continue;

            double? md = 0;
            for (int j = i; j < i + day; j++)
            {
                double? val = list.ElementAt(j).closePrice - list.ElementAt(i).MA20;
                md += val * val;
            }
            md = md / (day - 1);
            md = Math.Sqrt((double)md);
            list.ElementAt(i).BOLU = list.ElementAt(i).MA20 + k * md;
            list.ElementAt(i).BOLD = list.ElementAt(i).MA20 - k * md;
        }
    }

    public void UpdateBOLL(int day, int k)
    {
        if (list.Count < day)
            return;
  
        for (int i = 1; i >= 0; i--)
        {
            double? md = 0;
            for (int j = i; j < i + day; j++)
            {
                double? val = list.ElementAt(i).closePrice - list.ElementAt(i).MA20;
                md += val * val;
            }
            md = md / (day - 1);
            md = Math.Sqrt((double)md);
            list.ElementAt(i).BOLU = list.ElementAt(i).MA20 + k * md;
            list.ElementAt(i).BOLD = list.ElementAt(i).MA20 - k * md;
        }
    }
    public void CalcCurrentStochRSI14(int smoothenArg)
    {
        if (list.Count <= 14 || smoothenArg <= 0)
            return;

        for (int i = list.Count - 15; i >= 0; i--)
        {
            //K
            double? minRSIInPeriod, maxRSIInPeriod = minRSIInPeriod = list.ElementAt(i + 1).RSI14;
            for (int j = i + 2; j <= i + 14; j++)
            {
                if (minRSIInPeriod > list.ElementAt(j).RSI14)
                    minRSIInPeriod = list.ElementAt(j).RSI14;
                if (maxRSIInPeriod < list.ElementAt(j).RSI14)
                    maxRSIInPeriod = list.ElementAt(j).RSI14;
            }
            
            list.ElementAt(i).K = (list.ElementAt(i).RSI14 - minRSIInPeriod) / (maxRSIInPeriod - minRSIInPeriod) * 100;
            if (list.ElementAt(i).K > 100)
                list.ElementAt(i).K = 100;
            else if (list.ElementAt(i).K < 0)
                list.ElementAt(i).K = 0;

            //Smoothen
            if (i <= list.Count - 14)
            {
                double? sumK = 0;
                for (int j = i; j <= i + smoothenArg - 1; j++)
                    sumK += list.ElementAt(j).K;

                list.ElementAt(i).smoothenK = sumK / smoothenArg;
            }
        }

        stochRSI14GoBelow80 = list.First().smoothenK < 80 && list.ElementAt(1).smoothenK >= 80;
        stochRSI14GoBeyond20 = list.First().smoothenK > 20 && list.ElementAt(1).smoothenK <= 20;
    }

    public void UpdateStochRSI14(int smoothenArg)
    {
        if (list.Count <= 14 || smoothenArg <= 0)
            return;

        for (int i = 1; i >= 0; i--)
        {
            //K
            double? minRSIInPeriod, maxRSIInPeriod = minRSIInPeriod = list.ElementAt(i + 1).RSI14;
            for (int j = i + 2; j <= i + 14; j++)
            {
                if (minRSIInPeriod > list.ElementAt(j).RSI14)
                    minRSIInPeriod = list.ElementAt(j).RSI14;
                if (maxRSIInPeriod < list.ElementAt(j).RSI14)
                    maxRSIInPeriod = list.ElementAt(j).RSI14;
            }

            list.ElementAt(i).K = (list.ElementAt(i).RSI14 - minRSIInPeriod) / (maxRSIInPeriod - minRSIInPeriod) * 100;
            if (list.ElementAt(i).K > 100)
                list.ElementAt(i).K = 100;
            else if (list.ElementAt(i).K < 0)
                list.ElementAt(i).K = 0;

            //Smoothen
            double? sumK = 0;
            for (int j = i; j <= i + smoothenArg - 1; j++)
                sumK += list.ElementAt(j).K;

            list.ElementAt(i).smoothenK = sumK / smoothenArg;
        }
        stochRSI14GoBelow80 = list.First().smoothenK < 80 && list.ElementAt(1).smoothenK >= 80;
        stochRSI14GoBeyond20 = list.First().smoothenK > 20 && list.ElementAt(1).smoothenK <= 20;
    }
}