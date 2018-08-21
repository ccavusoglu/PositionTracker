namespace PositionTracker.Proxy.ProxyEntity
{
    public class ProxyPositionData
    {
        public string Coin { get; set; }
        public string Exchange { get; set; }
        public decimal Quantity { get; set; }
        public decimal Available { get; set; }
        public decimal Pending { get; set; }
        public decimal Balance { get; set; }
        public decimal BuyPrice { get; set; }
    }
}