using System.Collections.Generic;
using System.Linq;
using PositionTracker.Domain.Entity;
using PositionTracker.Event;
using PositionTracker.Event.Events;
using PositionTracker.Proxy.ProxyEntity;
using PositionTracker.Utility;

namespace PositionTracker.Core
{
    /// <summary>
    /// Manages user data. All user related data ops are being handled here.
    /// </summary>
    public class UserManager : IExitsGracefully
    {
        private readonly AvailableCoins availableCoins;
        private readonly User user;
        public ApiManager ApiManager { get; set; }
        public string UserTickerMarket => user.UserSettings.TickerMarket;

        public UserManager(AvailableCoins availableCoins)
        {
            this.availableCoins = availableCoins;
            user = new User();

            EventManager.Instance.Subscribe<UpdateSummaryEvent>(UpdateSummary);
        }

        public void ExitGracefully()
        {
            SaveUserData();
        }

        public async void FetchPositions()
        {
            await ApiManager.FetchPositions();

            EventManager.Instance.Fire(new FetchPositionsEvent());
        }

        public List<UserCoin> GetPositions()
        {
            var positions = new List<UserCoin>();

            foreach (var pos in user.UserPositions)
            {
                foreach (var item in pos.Value)
                {
                    positions.Add(item);
                }
            }

            return positions;
        }

        public decimal GetProfitPercentage(CoinMarketTicker ticker, UserCoin coin)
        {
            if (ticker == null || coin.BuyPrice == 0 ||
                ticker.Market != UserTickerMarket) return 0;

            return (ticker.Last - coin.BuyPrice) / coin.BuyPrice * 100m;
        }

        public UserSummary GetSummary()
        {
            return user.UserSummary;
        }

        public CoinMarketTicker GetTicker(string coin, string exchange)
        {
            if (availableCoins.Coins.ContainsKey(coin))
            {
                CoinMarketTicker coinMarketTicker;

                if (coin == UserTickerMarket)
                {
                    coinMarketTicker = availableCoins.Coins[coin].GetFirstTicker(exchange);
                }
                else
                {
                    coinMarketTicker = availableCoins.Coins[coin].GetTicker(UserTickerMarket, exchange);
                }

                return coinMarketTicker;
            }

            return null;
        }

        public decimal GetTotalPercentage(CoinMarketTicker ticker, UserCoin coin)
        {
            if (ticker == null || ticker.Market != UserTickerMarket) return 0;

            return (ticker.Last * coin.Quantity) / user.UserSummary.TotalBalance * 100m;
        }

        public void Init()
        {
            user.Load();

            foreach (var apiKey in user.UserSettings.ApiKeys)
            {
                ApiManager.SetApiKey(apiKey.Value.Exchange, apiKey.Value.Key, apiKey.Value.Secret);
            }
        }

        public void SaveUserData()
        {
            user.Save();
        }

        public void SetNotes(string coin, string exchange, string notes)
        {
            user.GetCoin(coin, exchange)?.SetNotes(notes);

            SaveUserData();
        }

        public void SetPositions(string exchange, IList<ProxyPositionData> positions)
        {
            var pos = user.UserPositions.GetOrAddEmpty(exchange);

            pos.Clear();

            foreach (var position in positions)
            {
                var userCoinKey = UserCoinKey.Get(position.Coin, position.Exchange);
                var userCoin = user.UserCoins.Get(userCoinKey);
                var coinTicker = GetTicker(position.Coin, position.Exchange)?.Last ?? 0m;

                var balance = position.Quantity * coinTicker;

                if (userCoin != null)
                {
                    userCoin.MergePosition(balance, position.Available, position.Pending, position.BuyPrice);
                }
                else
                {
                    userCoin = new UserCoin(position.Coin, position.Exchange,
                        balance, position.Available, position.Pending, position.BuyPrice, null);

                    user.UserCoins.TryAdd(userCoin.UniqueName, userCoin);
                }

                pos.Add(userCoin);
            }
        }

        public void SetTrades(string exchange, Dictionary<string, List<ProxyCoinOrderData>> trades)
        {
            var ts = user.UserTrades.GetOrAddEmpty(exchange);

            ts.Clear();

            foreach (var trade in trades)
            {
                var userTrade = ts.GetOrAddEmpty(trade.Key);

                foreach (var item in trade.Value) { userTrade.Add(DomainMapper.MapTrade(item)); }
            }
        }

        private void UpdateSummary(IEventBase obj)
        {
            var userSummary = user.UserSummary;

            var total = 0m;
            var exchangeTotals = new Dictionary<string, decimal>();

            foreach (var item in user.UserCoins)
            {
                var coin = availableCoins.Coins.Get(item.Value.Coin);

                if (coin == null)
                {
                    Logger.LogDebug($"UserCoin not fetched from any exchange: {item.Value.Coin}");

                    continue;
                }

                var coinTicker = coin.Tickers.Get(item.Value.Exchange);

                if (coin.Symbol == UserTickerMarket)
                {
                    userSummary.RemainingBtc[item.Value.Exchange] = item.Value.Quantity;
                    var btcTickers = coinTicker.MarketTickers.SingleOrDefault();

                    userSummary.BtcTickers[item.Value.Exchange] = btcTickers.Equals(default) ? null : btcTickers.Value;
                    total += item.Value.Quantity;

                    continue;
                }

                if (coinTicker == null)
                {
                    Logger.LogDebug($"UserCoin: {item.Value.Coin} Ticker not fetched from exchange: {item.Value.Exchange}");

                    continue;
                }

                if (coinTicker.MarketTickers.ContainsKey(UserTickerMarket))
                {
                    var ticker = coinTicker.MarketTickers[UserTickerMarket];
                    var temp = ticker.Last * item.Value.Quantity;

                    total += temp;

                    exchangeTotals.AddOrSum(item.Value.Exchange, temp);
                }
            }

            userSummary.SetTotalBalance(total);

            foreach (var exchangeTotal in exchangeTotals)
            {
                userSummary.SetBalance(exchangeTotal.Key, exchangeTotal.Value);
            }

            EventManager.Instance.Fire(new GetTickersEvent());
            EventManager.Instance.Fire(new GetSummaryEvent());
        }
    }
}