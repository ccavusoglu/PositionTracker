using System.Collections.Generic;

namespace PositionTracker.Domain.Entity
{
    public class AvailableCoins
    {
        public IDictionary<string, Coin> Coins { get; private set; }

        public AvailableCoins()
        {
            Coins = new Dictionary<string, Coin>();
        }

        public void Add(Coin coin)
        {
            if (Coins.ContainsKey(coin.Symbol))
                Coins[coin.Symbol].Merge(coin);
            else
                Coins.Add(coin.Symbol, coin);
        }

        public Coin Get(string symbol)
        {
            return Coins.ContainsKey(symbol) ? Coins[symbol] : null;
        }
    }
}