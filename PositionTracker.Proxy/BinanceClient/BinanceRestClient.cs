using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PositionTracker.Proxy.BinanceClient.Entity;
using PositionTracker.Proxy.BinanceClient.Response;
using PositionTracker.Proxy.ProxyEntity;
using PositionTracker.Proxy.Response;
using PositionTracker.Utility;

namespace PositionTracker.Proxy.BinanceClient
{
    public sealed class BinanceRestClient : BaseRestClient, IExchangeRestClient
    {
        private const string ApiHeader = "X-MBX-APIKEY";
        private readonly ISet<string> availableMarkets;
        private readonly string defaultMarket;
        private readonly IDictionary<string, IList<string>> markets;

        private int currentWeigth;
        private int lastWeigth;

        protected override string Name { get; set; }
        protected override int RequestLimitExceededWaitTimeMs { get; set; }
        protected override long RequestLimitPerMinute { get; set; }

        public BinanceRestClient()
        {
            RequestLimitPerMinute = Constant.BinanceRestRequestLimit;
            RequestLimitExceededWaitTimeMs = 60000;
            Name = Constant.Binance;
            markets = new Dictionary<string, IList<string>>();
            availableMarkets = new HashSet<string>();
            defaultMarket = Constant.Btc;

            var httpClientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };

            HttpClient = new HttpClient(httpClientHandler)
            {
                BaseAddress = new Uri(Constant.BinanceRestRootUrl)
            };

            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<PositionsResponse> FetchPositions(bool onlyDefaultMarket)
        {
            var e = LogExecutionTime.Begin();

            var req = CreateRequest(HttpMethod.Get, BinanceEndpoints.Account);
            var res = await GetResponse<BinanceResponses.GetBalances>(req, BinanceEndpoints.Account);

            if (!IsResultSuccess<PositionsResponse>(res, out var response) || res?.Result == null || res.Result.Count == 0)
            {
                Logger.LogProxy($"Binance GetBalances Error. Code: {response.ErrorCode} Message: {response.ErrorMessage}");

                var positionsResponse = await GetCachedResponse<PositionsResponse>(BinanceEndpoints.Account);

                return positionsResponse.IsCached ? positionsResponse : response;
            }

            var orders = new Dictionary<string, List<ProxyCoinOrderData>>();
            var temp = new List<ProxyPositionData>();

            e.LogInterval("GetBalances");

            foreach (var balance in res.Result)
            {
                if (balance.Quantity == 0) continue;

                if (markets.ContainsKey(balance.Symbol))
                {
                    orders.Add(balance.Symbol, new List<ProxyCoinOrderData>());

                    foreach (var market in markets[balance.Symbol])
                    {
                        if (onlyDefaultMarket && market != defaultMarket) continue;

                        var result = await GetMyTrades(balance.Symbol, market);

                        if (!result.IsSuccess) continue;

                        orders[balance.Symbol].AddRange(result.Trades);
                    }

                    balance.Balance = balance.Quantity;

                    ParseCoinPosition(balance, orders[balance.Symbol]);
                }

                temp.Add(new ProxyPositionData
                {
                    Coin = balance.Symbol,
                    Exchange = Constant.Binance,
                    Quantity = balance.Quantity,
                    Available = balance.Available,
                    Pending = balance.Pending,
                    BuyPrice = balance.AvgBuyPrice
                });
            }

            e.End();

            response.Positions = temp;
            response.Trades = orders;

            return response;
        }

        public async Task<AvailableCoinsResponse> GetExchangeInfo()
        {
            var req = CreateRequest(HttpMethod.Get, BinanceEndpoints.ExchangeInfo);
            var res = await GetResponse<BinanceResponses.GetExchangeInfo>(req, BinanceEndpoints.ExchangeInfo);

            if (!IsResultSuccess<AvailableCoinsResponse>(res, out var response) || res?.Coins == null)
            {
                Logger.LogProxy(
                    $"Binance GetExchangeInfo Error. Code: {response.ErrorCode} Message: {response.ErrorMessage}");

                var availableCoinsResponse = await GetCachedResponse<AvailableCoinsResponse>(BinanceEndpoints.ExchangeInfo);

                return availableCoinsResponse.IsCached ? availableCoinsResponse : response;
            }

            var temp = new List<ProxyCoinInfoData>();

            BinanceEndpoints.TickerAndStats.Weight = res.Coins.Count;

            SetRateLimits(res.RateLimits);
            TimestampOffset = Util.UnixTimestampToDateTime(res.ServerTime) - DateTime.UtcNow;

            markets.Clear();
            availableMarkets.Clear();

            foreach (var coin in res.Coins)
            {
                var mapped = ProxyMapper.MapBinanceCoinInfo(coin);

                if (mapped == null) continue;

                temp.Add(mapped);

                markets.AddOrAppend(mapped.Symbol, mapped.Market, (key, value) => markets[key].Add(value));
                availableMarkets.Add(mapped.Market);
            }

            response.Coins = temp;

            CacheResponse(BinanceEndpoints.ExchangeInfo, typeof(AvailableCoinsResponse), response);

            return response;
        }

        public async Task<MyTradesResponse> GetMyTrades(string coin, string market)
        {
            var e = LogExecutionTime.Begin();

            var req = CreateRequest(HttpMethod.Get, BinanceEndpoints.MyTrades, $"symbol={coin}{market}&limit=25");
            var res = await GetResponse<List<BinanceMyTrade>>(req, BinanceEndpoints.MyTrades);

            if (res == null || res.Count == 0)
            {
                Logger.LogProxy($"Binance {coin} trades returned empty! Url: {req.RequestUri.AbsoluteUri}", res);

                return new MyTradesResponse
                {
                    IsSuccess = false,
                    ErrorMessage = $"Binance {coin} trades returned empty! Url: {req.RequestUri.AbsoluteUri}"
                };
            }

            e.End(customMessage: $"{coin}-{market} at Binance ");

            return new MyTradesResponse
            {
                IsSuccess = true,
                Trades = ProxyMapper.MapCoinTrade(res, coin)
            };
        }

        public async Task<CoinsTickerResponse> GetTickers()
        {
            var e = LogExecutionTime.Begin();

            var req = CreateRequest(HttpMethod.Get, BinanceEndpoints.TickerAndStats);

            var res = await GetResponse<List<BinanceTicker>>(req, BinanceEndpoints.MyTrades);

            if (res == null || res.Count == 0)
            {
                Logger.LogProxy($"Binance tickers returned empty! Url: {req.RequestUri.AbsoluteUri}", res);

                return new CoinsTickerResponse
                {
                    IsSuccess = false,
                    ErrorMessage = $"Binance tickers returned empty! Url: {req.RequestUri.AbsoluteUri}"
                };
            }

            var temp = new List<ProxyCoinTickerData>();

            foreach (var c in res)
            {
                c.Coin = ParseBinanceSymbol(c.Symbol, out c.Market);

                var coinTicker = ProxyMapper.MapBinanceTicker(c);

                if (coinTicker.Coin != null) temp.Add(coinTicker);
            }

            e.End();

            return new CoinsTickerResponse
            {
                IsSuccess = true,
                Tickers = temp
            };
        }

        public string ParseBinanceSymbol(string symbol, out string market)
        {
            var marketIndex = -1;
            market = null;

            var lookupMarkets = new List<string>();

            lookupMarkets.AddRange(!markets.ContainsKey(symbol) ? availableMarkets.ToList() : markets[symbol]);

            foreach (var m in lookupMarkets)
            {
                if (symbol.EndsWith(m, StringComparison.Ordinal))
                {
                    marketIndex = symbol.IndexOf(m, StringComparison.Ordinal);

                    break;
                }
            }

            if (marketIndex == 0 || marketIndex == -1) return null;

            market = symbol.Substring(marketIndex);

            return symbol.Substring(0, symbol.Length - market.Length);
        }

        public override void SetApiKey(string apiKey, string apiToken)
        {
            ApiKey = apiKey;
            ApiSecret = apiToken;

            lock (LockObject)
            {
                if (HttpClient.DefaultRequestHeaders.Contains(ApiHeader))
                {
                    if (HttpClient.DefaultRequestHeaders.Contains(ApiHeader))
                        HttpClient.DefaultRequestHeaders.Remove(ApiHeader);
                }

                HttpClient.DefaultRequestHeaders.TryAddWithoutValidation(ApiHeader, new[] {apiKey});
            }
        }

        public void ThrottleIfNecessary()
        {
            if (PreventThrottle) return;

            var reset = false;

            if (currentWeigth > RequestLimitPerMinute)
            {
                RequestThrottleResetEvent.Reset();

                RequestMinRemainingMs = (RequestLimitIntervalStartTime.AddMinutes(1).Ticks - DateTime.Now.Ticks) /
                                        TimeSpan.TicksPerMillisecond;

                Logger.LogFatal($"Binance Rate Limit Exceeded. Throttling request for {RequestMinRemainingMs}ms");

                if (RequestMinRemainingMs > 0)
                    Thread.Sleep((int) RequestMinRemainingMs);

                RequestThrottleResetEvent.Set();

                reset = true;
            }

            if (reset || DateTime.Now > RequestLimitIntervalStartTime.AddMinutes(1))
            {
                currentWeigth = lastWeigth;
                RequestLimitIntervalStartTime = DateTime.Now;
            }
        }

        // TODO: move to interceptor
        private void CacheResponse(BinanceEndpointData endpoint, Type type, BaseResponse response)
        {
            if (endpoint.IsCacheable)
            {
                response.IsCached = true;
                ResponseCache[type] = response;
            }
        }

        private HttpRequestMessage CreateRequest(HttpMethod httpMethod, BinanceEndpointData endpoint, string args = null)
        {
            if (endpoint.IsSigned)
            {
                var timestamp = DateTimeOffset.UtcNow.AddMilliseconds(TimestampOffset.TotalMilliseconds)
                    .ToUnixTimeMilliseconds()
                    .ToString();
                var argEnding = $"recvWindow=60000&timestamp={timestamp}";
                var adjustedSignature = !string.IsNullOrEmpty(args) ? $"{args}&{argEnding}" : $"{argEnding}";

                var signature = CreateSignature(adjustedSignature);

                var completeUri =
                    new Uri($"{Constant.BinanceRestRootUrl}{endpoint.Endpoint}?{adjustedSignature}&signature={signature}");

                var request = new HttpRequestMessage(httpMethod, completeUri);

                return request;
            }
            else
            {
                var completeUri = $"{Constant.BinanceRestRootUrl}{endpoint.Endpoint}?{args}";

                var request = new HttpRequestMessage(httpMethod, completeUri);

                return request;
            }
        }

        private string CreateSignature(string message)
        {
            var keyBytes = Encoding.UTF8.GetBytes(ApiSecret);
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var hash = new HMACSHA256(keyBytes);
            var bytes = hash.ComputeHash(messageBytes);

            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        private Task<T> GetCachedResponse<T>(BinanceEndpointData endpoint) where T : class, new()
        {
            return Task.Run(() =>
            {
                var val = ResponseCache.Get(typeof(T)) as T;

                if (!endpoint.IsCacheable || val == null) return new T();

                return val;
            });
        }

        private async Task<T> GetResponse<T>(HttpRequestMessage req, BinanceEndpointData endpoint) where T : class, new()
        {
            if (RequestLimitExceeded) return null;

            currentWeigth += endpoint.Weight;
            lastWeigth = endpoint.Weight;

            if (!endpoint.ShouldByPassThrottle)
            {
                RequestThrottleResetEvent.WaitOne();
                ThrottleIfNecessary();
            }

            string content = null;

            try
            {
                HttpResponseMessage response = null;

                if (req.Method == HttpMethod.Get)
                    response = await HttpClient.GetAsync(req.RequestUri);
                else if (req.Method == HttpMethod.Post)
                    response = await HttpClient.PostAsync(req.RequestUri, req.Content);
                else
                    throw new ArgumentOutOfRangeException(req.Method.ToString());

                content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == (HttpStatusCode) 418)
                    {
                        Logger.LogFatal("Binance Rate LIMIT EXCEEDED! IP Banned.");

                        SetRequestLimitExceeded();
                    }
                    else if (response.StatusCode == (HttpStatusCode) 429)
                    {
                        Logger.LogFatal("Binance Rate LIMIT EXCEEDED!");

                        SetRequestLimitExceeded();
                    }
                    else
                        Logger.LogProxy(
                            $"Url: {req.RequestUri.AbsoluteUri} Status: {response.StatusCode} Content: {response.Content} Error: {content}");

                    return null;
                }

                RequestLimitExceeded = false;

                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception ex)
            {
                Logger.LogError($"UnknownError fetching Url: {req.RequestUri.AbsoluteUri} Content: {content}", ex);

                return null;
            }
        }

        private bool IsResultSuccess<T>(BinanceResponses.BinanceResponse res, out T response) where T : BaseResponse, new()
        {
            response = new T
            {
                Exchange = Constant.Binance,
                TimeStamp = DateTime.Now,
                IsSuccess = true
            };

            if (res == null || res.ErrorCode != 0 || !string.IsNullOrEmpty(res.Message))
            {
                response.ErrorCode = res?.ErrorCode.ToString();
                response.ErrorMessage = res?.Message;
                response.IsSuccess = false;
            }

            return response.IsSuccess;
        }

        private void ParseCoinPosition(BinanceBalance balance, IList<ProxyCoinOrderData> orders)
        {
            var buys = orders.Where(x => x.OrderSide == ProxyEnum.CoinOrderSide.BUY).ToList();

            var qty = 0m;
            var total = 0m;
            var loop = false;
            var remaining = balance.Balance;

            for (var i = buys.Count - 1; i >= 0; i--)
            {
                var buy = buys[i];

                qty += buy.QuantityBalance;

                if (!loop)
                {
                    if (qty == balance.Balance)
                    {
                        // all bought at the last order
                        balance.AvgBuyPrice = buy.Price;
                        balance.Balance = buy.QuantityBalance;

                        break;
                    }

                    if (qty > balance.Balance)
                    {
                        balance.AvgBuyPrice = buy.Price;

                        break;
                    }
                }

                if (remaining <= 0) break;

                if (qty < balance.Balance)
                {
                    loop = true;

                    total += buy.QuantityBalance * buy.Price;

                    balance.AvgBuyPrice = total / qty;

                    remaining -= buy.QuantityBalance;
                }
                else
                {
                    total += Math.Min(remaining, qty) * buy.Price;

                    balance.AvgBuyPrice = total / balance.Balance;

                    remaining -= buy.QuantityBalance;
                }
            }
        }

        private void SetRateLimits(List<BinanceRateLimits> limits)
        {
            if (limits != null && limits.Count > 0)
            {
                foreach (var limit in limits)
                {
                    if (limit.RateLimitType == "REQUESTS")
                    {
                        if (limit.Interval == "MINUTE")
                            RequestLimitPerMinute = limit.Limit;

                        break;
                    }
                }
            }
        }

        private void SetRequestLimitExceeded()
        {
            if (!RequestLimitExceeded)
            {
                RequestLimitExceeded = true;

                Task.Delay(RequestLimitExceededWaitTimeMs)
                    .ContinueWith(task => RequestLimitExceeded = false);
            }
        }
    }
}