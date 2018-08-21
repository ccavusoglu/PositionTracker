using System;

namespace PositionTracker.Proxy.ProxyEntity
{
    public class ProxyCoinOrderData
    {
        public string Id { get; set; }
        public string Symbol { get; set; }
        public string Market { get; set; }
        public string Exchange { get; set; }
        public ProxyEnum.CoinOrderSide OrderSide { get; set; }
        public ProxyEnum.CoinOrderStatus OrderStatus { get; set; }
        public decimal Quantity { private get; set; }
        public decimal QuantityRemaining { get; set; }
        public decimal QuantityBalance => Quantity - QuantityRemaining;
        public decimal Commission { get; set; }
        public decimal Price { get; set; }
        public ProxyEnum.CoinOrderCondition Condition { get; set; }
        public decimal? ConditionTarget { get; set; }
        public DateTime Time { get; set; }
    }
}