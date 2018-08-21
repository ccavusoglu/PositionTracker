using System;

namespace PositionTracker.Proxy.ProxyEntity
{
    public class ProxyCoinTickerData
    {
        public string Exchange { get; set; }
        public string Coin { get; set; }
        public string Market { get; set; }
        public decimal Ask { get; set; }
        public decimal Bid { get; set; }
        public decimal Last { get; set; }

        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Volume { get; set; }
        public decimal PrevDayLast { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}