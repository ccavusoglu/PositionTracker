using System.Collections.Generic;
using System.Linq;
using PositionTracker.Utility;

namespace PositionTracker.Domain.Entity
{
    public class Coin
    {
        public string Symbol { get; private set; }
        public string Name { get; private set; }
        public HashSet<string> Exchanges { get; private set; }
        public IDictionary<string, IList<string>> Markets { get; private set; }
        public IDictionary<string, CoinExchangeSettings> Settings { get; private set; }
        public IDictionary<string, CoinTicker> Tickers { get; private set; }

        public Coin(string symbol, string name, string exchange, string market, CoinExchangeSettings exchangeSettings)
        {
            Symbol = symbol;
            Name = name;
            Exchanges = new HashSet<string> {exchange};
            Markets = new Dictionary<string, IList<string>> {{exchange, new List<string> {market}}};
            Settings = new Dictionary<string, CoinExchangeSettings> {{exchange, exchangeSettings}};
            Tickers = new Dictionary<string, CoinTicker>();
        }

        public CoinMarketTicker GetFirstTicker(string exchange)
        {
            if (Tickers.ContainsKey(exchange))
            {
                if (Tickers[exchange].MarketTickers.Count > 0)
                {
                    return Tickers[exchange].MarketTickers.First().Value;
                }
            }

            return null;
        }

        public CoinMarketTicker GetTicker(string market, string exchange)
        {
            if (Tickers.ContainsKey(exchange))
            {
                if (Tickers[exchange].MarketTickers.ContainsKey(market))
                {
                    return Tickers[exchange].MarketTickers[market];
                }
            }

            return null;
        }

        public void Merge(Coin coin)
        {
            foreach (var coinExchange in coin.Exchanges) { Exchanges.Add(coinExchange); }

            foreach (var coinMarket in coin.Markets)
            {
                foreach (var item in coinMarket.Value)
                {
                    Markets.AddOrAppend(coinMarket.Key, item, (key, value) => { Markets[coinMarket.Key].Add(item); });
                }
            }

            foreach (var coinExchangeSettingse in coin.Settings)
            {
                Settings[coinExchangeSettingse.Key] = coinExchangeSettingse.Value;
            }
        }

        public void Tick(CoinTicker ticker)
        {
            if (!Tickers.ContainsKey(ticker.Exchange))
            {
                Tickers.Add(ticker.Exchange, new CoinTicker(ticker.Coin, ticker.Exchange, ticker.MarketTickers));
            }

            var exchangeTicker = Tickers[ticker.Exchange];

            exchangeTicker.MergeSingle(ticker);
        }
    }
}