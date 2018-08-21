using System.Collections.Generic;
using Newtonsoft.Json;

namespace PositionTracker.Proxy.BinanceClient.Entity
{
    public class BinanceCoinInfo
    {
        [JsonProperty("filters")] public List<BinanceCoinFilter> Filters;
        [JsonProperty("quoteAsset")] public string Market;
        [JsonProperty("orderTypes")] public string[] OrderTypes;
        [JsonProperty("quotePrecision")] public int PricePrecision;
        [JsonProperty("baseAssetPrecision")] public int QuantityPrecision;
        [JsonProperty("status")] public string Status;
        [JsonProperty("baseAsset")] public string Symbol;
    }
}