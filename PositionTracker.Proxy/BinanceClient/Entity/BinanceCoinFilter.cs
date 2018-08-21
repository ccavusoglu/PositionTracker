using Newtonsoft.Json;

namespace PositionTracker.Proxy.BinanceClient.Entity
{
    public class BinanceCoinFilter
    {
        [JsonProperty("filterType")] public string FilterType;
        [JsonProperty("maxPrice")] public string MaxPrice;
        [JsonProperty("maxQty")] public string MaxQty;
        [JsonProperty("minNotional")] public string MinNotional;
        [JsonProperty("minPrice")] public string MinPrice;
        [JsonProperty("minQty")] public string MinQty;
        [JsonProperty("stepSize")] public string StepSize;
        [JsonProperty("tickSize")] public string TickSize;
    }
}