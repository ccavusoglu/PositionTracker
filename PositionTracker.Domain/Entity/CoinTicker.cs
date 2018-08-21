using System;
using System.Collections.Generic;
using System.Linq;
using PositionTracker.Utility;

namespace PositionTracker.Domain.Entity
{
    public class CoinMarketTicker
    {
        public string Coin { get; internal set; }
        public string Exchange { get; internal set; }
        public string Market { get; internal set; }
        public decimal Ask { get; internal set; }
        public decimal Bid { get; internal set; }
        public decimal Last { get; internal set; }
        public decimal High { get; internal set; }
        public decimal Low { get; internal set; }
        public decimal PrevDayLast { get; internal set; }
        public decimal Volume { get; internal set; }
        public DateTime TimeStamp { get; internal set; }
        public string TickerSymbol { get; internal set; }
        public decimal PriceChange24Hr => PrevDayLast == 0 ? 0 : ((Last - PrevDayLast) / PrevDayLast) * 100m;

        public CoinMarketTicker(string coin, string exchange, string market, string tickerSymbol,
            decimal ask, decimal bid, decimal last, decimal high,
            decimal low, decimal prevDayLast, decimal volume, DateTime timeStamp)
        {
            Coin = coin;
            Exchange = exchange;
            Market = market;
            Ask = ask;
            Bid = bid;
            Last = last;
            High = high;
            Low = low;
            PrevDayLast = prevDayLast;
            Volume = volume;
            TimeStamp = timeStamp;
            TickerSymbol = tickerSymbol;
        }
    }

    public class CoinTicker
    {
        public IDictionary<string, CoinMarketTicker> MarketTickers;
        public string Coin { get; internal set; }
        public string Exchange { get; internal set; }

        public CoinTicker(string coin, string exchange, IDictionary<string, CoinMarketTicker> marketTicker)
        {
            Coin = coin;
            Exchange = exchange;
            MarketTickers = marketTicker;
        }

        public CoinTicker(string coin, string exchange, CoinMarketTicker marketTicker)
        {
            if (MarketTickers == null) MarketTickers = new Dictionary<string, CoinMarketTicker>();

            MarketTickers.Add(marketTicker.Market, marketTicker);

            Coin = coin;
            Exchange = exchange;
        }

        public void MergeSingle(CoinTicker ticker)
        {
            Coin = ticker.Coin;
            Exchange = ticker.Exchange;

            var theirTicker = ticker.MarketTickers.First().Value;
            var marketTicker = MarketTickers.Get(theirTicker.Market);

            if (marketTicker != null)
            {
                marketTicker.Market = theirTicker.Market;
                marketTicker.Ask = theirTicker.Ask;
                marketTicker.Bid = theirTicker.Bid;
                marketTicker.Last = theirTicker.Last;
                marketTicker.High = theirTicker.High;
                marketTicker.Low = theirTicker.Low;
                marketTicker.PrevDayLast = theirTicker.PrevDayLast;
                marketTicker.Volume = theirTicker.Volume;
                marketTicker.TimeStamp = theirTicker.TimeStamp;
                marketTicker.TickerSymbol = theirTicker.TickerSymbol;
            }
            else
            {
                MarketTickers.Add(theirTicker.Market, new CoinMarketTicker(
                    Coin, Exchange, theirTicker.Market, theirTicker.TickerSymbol, theirTicker.Ask,
                    theirTicker.Bid, theirTicker.Last, theirTicker.High, theirTicker.Low, theirTicker.PrevDayLast,
                    theirTicker.Volume, theirTicker.TimeStamp));
            }
        }
    }
}