using System;
using Newtonsoft.Json;

namespace PositionTracker.Proxy.BinanceClient.Entity
{
    [Serializable]
    public class BinanceBalance
    {
        [JsonProperty("asset")]
        public string Symbol { get; set; }

        public decimal Balance { get; set; }

        public decimal Quantity => Available + Pending;

        [JsonProperty("locked")]
        public decimal Pending { get; set; }

        [JsonProperty("free")]
        public decimal Available { get; set; }

        public decimal AvgBuyPrice { get; set; }
    }
}