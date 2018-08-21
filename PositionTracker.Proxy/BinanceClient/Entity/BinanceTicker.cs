using System;
using Newtonsoft.Json;

namespace PositionTracker.Proxy.BinanceClient.Entity
{
    [Serializable]
    public class BinanceTicker
    {
        [JsonProperty("askPrice")] public decimal Ask;
        [JsonProperty("bidPrice")] public decimal Bid;
        [JsonIgnore] public string Coin;
        [JsonProperty("highPrice")] public decimal High;
        [JsonProperty("lastPrice")] public decimal Last;
        [JsonProperty("lowPrice")] public decimal Low;
        [JsonIgnore] public string Market;
        [JsonProperty("prevClosePrice")] public decimal PrevLastPrice;
        [JsonProperty("price")] public decimal Price;
        [JsonProperty("priceChangePercent")] public decimal PriceChangePercent;
        [JsonProperty("symbol")] public string Symbol;
        [JsonProperty("quoteVolume")] public decimal Volume;
    }
}