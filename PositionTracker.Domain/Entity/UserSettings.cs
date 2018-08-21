using System.Collections.Generic;
using Newtonsoft.Json;
using PositionTracker.Utility;

namespace PositionTracker.Domain.Entity
{
    public class UserSettings
    {
        public string TickerMarket { get; set; } = Constant.Btc;
        public IDictionary<string, ApiKey> ApiKeys { get; internal set; }

        public UserSettings()
        {
            ApiKeys = new Dictionary<string, ApiKey>();
        }
    }

    public class ApiKey
    {
        public string Exchange { get; internal set; }
        public string Key { get; internal set; }
        public string Secret { get; internal set; }

        [JsonConstructor]
        public ApiKey(string exchange, string key, string secret)
        {
            Exchange = exchange;
            Key = key;
            Secret = secret;
        }
    }
}