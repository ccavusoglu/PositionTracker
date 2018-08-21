namespace PositionTracker.Domain.Entity
{
    public class UserTrade
    {
        public Enum.OrderSide OrderSide;
        public decimal Price;
        public decimal Quantity;
        public string Symbol { get; }
        public string EXchange { get; }

        public UserTrade(string symbol, string exchange, decimal quantity, decimal price, Enum.OrderSide orderSide)
        {
            Symbol = symbol;
            EXchange = exchange;
            Quantity = quantity;
            Price = price;
            OrderSide = orderSide;
        }
    }
}