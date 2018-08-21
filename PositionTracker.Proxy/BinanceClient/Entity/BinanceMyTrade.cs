using Newtonsoft.Json;

namespace PositionTracker.Proxy.BinanceClient.Entity
{
    public class BinanceMyTrade
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("qty")]
        public decimal Quantity { get; set; }

        [JsonProperty("commission")]
        public decimal Comminission { get; set; }

        [JsonProperty("commissionAsset")]
        public string ComminissionCurrency { get; set; }

        [JsonProperty("isBuyer")]
        public bool IsBuyer { get; set; }

        [JsonProperty("isMaker")]
        public bool IsMaker { get; set; }

        [JsonProperty("isBestMatch")]
        public bool IsBestMatch { get; set; }

        [JsonProperty("time")]
        public long Time { get; set; }
    }
}