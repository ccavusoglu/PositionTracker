namespace PositionTracker.Domain.Entity
{
    public class CoinExchangeSettings
    {
        public string Exchange { get; }
        public decimal MinimumOrderAmount { get; }
        public int PricePrecisions { get; }
        public int QuantityPrecisions { get; }

        public CoinExchangeSettings(string exchange, decimal minimumOrderAmount, int pricePrecisions, int quantityPrecisions)
        {
            Exchange = exchange;
            MinimumOrderAmount = minimumOrderAmount;
            PricePrecisions = pricePrecisions;
            QuantityPrecisions = quantityPrecisions;
        }
    }
}