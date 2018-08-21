using Newtonsoft.Json;

namespace PositionTracker.Proxy.BinanceClient.Entity
{
    public class BinanceRateLimits
    {
        [JsonProperty("interval")] public string Interval;
        [JsonProperty("limit")] public int Limit;
        [JsonProperty("rateLimitType")] public string RateLimitType;
    }
}