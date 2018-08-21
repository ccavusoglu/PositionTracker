using System;

namespace PositionTracker.Utility
{
    public static class Util
    {
        public static string GetCurrencySymbol(string market)
        {
            switch (market)
            {
                case "BTC":
                    return Constant.BtcSym;
                case "USD":
                case "USDT":
                case "TUSD":
                    return Constant.UsdSym;
                case "TRY":
                    return Constant.TrySym;
            }

            return market;
        }

        public static long GetTimestamp()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public static string ToCustomTimeString(this DateTime dt)
        {
            return dt.ToString("HH:mm:ss:fff");
        }

        public static DateTime UnixTimestampToDateTime(long unixTimestamp)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(unixTimestamp).DateTime;
        }
    }
}