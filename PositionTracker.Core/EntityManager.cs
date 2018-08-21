using System.Collections.Generic;
using PositionTracker.Domain.Entity;
using PositionTracker.Proxy.ProxyEntity;

namespace PositionTracker.Core
{
    public class EntityManager
    {
        private readonly AvailableCoins availableCoins;

        public EntityManager(AvailableCoins availableCoins)
        {
            this.availableCoins = availableCoins;
        }

        public void SetAvailableCoins(IList<ProxyCoinInfoData> coins)
        {
            foreach (var coinInfo in coins)
            {
                availableCoins.Add(DomainMapper.MapCoin(coinInfo));
            }
        }

        public void UpdateTickers(List<ProxyCoinTickerData> tickers)
        {
            foreach (var ticker in tickers)
            {
                var coin = availableCoins.Get(ticker.Coin);

                coin?.Tick(DomainMapper.MapCoinTicker(ticker));
            }
        }
    }
}