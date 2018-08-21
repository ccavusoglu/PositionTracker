using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PositionTracker.Proxy.BinanceClient.Entity;

namespace PositionTracker.Proxy.BinanceClient.Response
{
    public class BinanceResponses
    {
        [Serializable]
        public class BinanceResponse
        {
            [JsonProperty("code")]
            public long ErrorCode { get; set; }

            [JsonProperty("msg")]
            public string Message { get; set; }
        }

        [Serializable]
        public class GetExchangeInfo : BinanceResponse
        {
            [JsonProperty("symbols")] public List<BinanceCoinInfo> Coins;
            [JsonProperty("rateLimits")] public List<BinanceRateLimits> RateLimits;
            [JsonProperty("serverTime")] public long ServerTime;
            [JsonProperty("timezone")] public string ServerTimezone;
        }

        [Serializable]
        public class GetTicker : BinanceResponse
        {
            [JsonProperty("price")] public BinanceTicker Result;
        }

        [Serializable]
        public class GetBalances : BinanceResponse
        {
            [JsonProperty("balances")] public IList<BinanceBalance> Result;
        }

        [Serializable]
        public class PlaceOrder : BinanceResponse
        {
            [JsonProperty("symbol")]
            public string Symbol { get; set; }

            [JsonProperty("origClientOrderId")]
            public string OrigClientOrderId { get; set; }

            [JsonProperty("orderId")]
            public long OrderId { get; set; }

            [JsonProperty("clientOrderId")]
            public string ClientOrderId { get; set; }

            [JsonProperty("transactTime")]
            public long TimeStamp { get; set; }
        }

        [Serializable]
        public class GetOrderBook : BinanceResponse
        {
            [JsonProperty("lastUpdateId")]
            public long LastUpdateId { get; set; }

            [JsonProperty("bids")]
            public dynamic Bids { get; set; }

            [JsonProperty("asks")]
            public dynamic Asks { get; set; }
        }

        [Serializable]
        public class SocketMessage
        {
            [JsonProperty("stream")]
            public string Stream { get; set; }
        }

        [Serializable]
        public class SocketTickerMessage
        {
            [JsonProperty("stream")]
            public string Stream { get; set; }

            [JsonProperty("data")]
            public IList<SocketTickers> Tickers { get; set; }
        }

        [Serializable]
        public class SocketTickers
        {
            [JsonProperty("a")] public string Ask; // Best ask price
            [JsonProperty("A")] public string AskQty; // Best ask quantity
            [JsonProperty("b")] public string Bid; // Best bid price
            [JsonProperty("B")] public string BidQty; // Bid bid quantity
            [JsonProperty("E")] public long EventTime; // Event time
            [JsonProperty("e")] public string EventType; //"24hrTicker", Event type
            [JsonProperty("F")] public long FirstTradeId; // First trade ID
            [JsonProperty("h")] public string High; // High price
            [JsonProperty("c")] public string LastPrice; // Current day's close price
            [JsonProperty("L")] public long LastTradeId; // Last trade Id
            [JsonProperty("Q")] public string LastTradesQty; // Close trade's quantity
            [JsonProperty("l")] public string Low; // Low price
            [JsonProperty("o")] public string Open; // Open price
            [JsonProperty("x")] public string PrevDaysClose; // Previous day's close price
            [JsonProperty("p")] public string PriceChange; // Price change
            [JsonProperty("P")] public string PriceChangePercent; // Price change percent
            [JsonProperty("C")] public long StatsCloseTime; // Statistics close time
            [JsonProperty("O")] public long StatsOpenTime; // Statistics open time
            [JsonProperty("s")] public string Symbol; // Coin
            [JsonProperty("n")] public long TotalTradesQty; // Total number of trades
            [JsonProperty("v")] public string Volume; // Total traded base asset volume
            [JsonProperty("q")] public string VolumeInQuote; // Total traded quote asset volume
            [JsonProperty("w")] public string WeightAvgPrice; // Weighted average price
        }
    }
}