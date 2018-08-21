namespace PositionTracker.Proxy.BinanceClient
{
    public class BinanceEndpointData
    {
        public string Endpoint;
        public bool IsCacheable;
        public bool IsSigned;
        public bool ShouldByPassThrottle;
        public int Weight;

        public BinanceEndpointData(string endpoint, bool isSigned = false, int weight = 1, bool isCacheable = false,
            bool byPassThrottle = false)
        {
            Endpoint = endpoint;
            IsSigned = isSigned;
            Weight = weight;
            IsCacheable = isCacheable;
            ShouldByPassThrottle = byPassThrottle;
        }
    }
}