namespace PositionTracker.Proxy.ProxyEntity
{
    public class ProxyEnum
    {
        public enum CoinOrderCondition
        {
            NONE,
            LESS_THAN,
            GREATER_THAN
        }

        public enum CoinOrderSide
        {
            BUY,
            SELL
        }

        public enum CoinOrderStatus
        {
            NONE,
            FILLED,
            PARTIALLY_FILLED,
            OPEN
        }
    }
}