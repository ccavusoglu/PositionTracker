using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PositionTracker.Core.Attributes;
using PositionTracker.Proxy;
using PositionTracker.Proxy.BinanceClient;
using PositionTracker.Utility;

namespace PositionTracker.Core
{
    /// <summary>
    /// Layer above rest APIs. Handles rest api requests, responses.
    /// </summary>
    public class ApiManager
    {
        private readonly EntityManager entityManager;
        private readonly IDictionary<string, IExchangeRestClient> exchangeClients;
        public UserManager UserManager { get; set; }

        public ApiManager(EntityManager entityManager, BinanceRestClient binanceRestClient)
        {
            this.entityManager = entityManager;
            exchangeClients = new Dictionary<string, IExchangeRestClient> {{Constant.Binance, binanceRestClient}};
        }

        [ExecutionTimeLog]
        public virtual async Task FetchPositions(bool onlyDefaultMarket = true)
        {
            foreach (var exchangeClient in exchangeClients)
            {
                var res = await exchangeClient.Value.FetchPositions(onlyDefaultMarket);

                if (res.IsSuccess && res.Positions != null)
                {
                    UserManager.SetPositions(exchangeClient.Key, res.Positions);
                    UserManager.SetTrades(exchangeClient.Key, res.Trades);
                }
                else
                {
                    Logger.LogFatal($"FetchPositions returned empty: {exchangeClient.Key}");
                }

                Logger.LogDebug($"FetchPositions for: {exchangeClient.Key}");
            }
        }

        [ExecutionTimeLog]
        public virtual async Task GetAvailableCoins()
        {
            foreach (var exchangeClient in exchangeClients)
            {
                var res = await exchangeClient.Value.GetExchangeInfo();

                if (res.IsSuccess && res.Coins != null)
                {
                    entityManager.SetAvailableCoins(res.Coins);
                }
                else
                {
                    Logger.LogFatal($"Response returned empty: {exchangeClient.Key}");
                }

                Logger.LogDebug($"Available Coins Fetched for: {exchangeClient.Key}");
            }
        }

        public async Task GetTickers()
        {
            foreach (var exchangeClient in exchangeClients)
            {
                var res = await exchangeClient.Value.GetTickers();

                if (res.IsSuccess && res.Tickers != null)
                {
                    entityManager.UpdateTickers(res.Tickers);
                }
                else
                {
                    Logger.LogFatal($"GetTickers returned empty: {exchangeClient.Key}");
                }
            }
        }

        public async void Init()
        {
            await GetAvailableCoins();
            await FetchPositions();
            await GetTickers();
        }

        public void SetApiKey(string exchange, string key, string secret)
        {
            ((BaseRestClient) exchangeClients[exchange]).SetApiKey(key, secret);
        }
    }
}