using Newtonsoft.Json;

namespace PositionTracker.WebUI.Models
{
    public class LiveDataModel
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        public LiveDataModel(string type, object obj)
        {
            Type = type;
            Data = obj;
        }
    }
}