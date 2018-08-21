using System;
using System.Collections.Generic;
using PositionTracker.Proxy.BinanceClient.Entity;
using PositionTracker.Proxy.ProxyEntity;
using PositionTracker.Utility;

namespace PositionTracker.Proxy
{
    public class ProxyMapper
    {
        public static ProxyCoinInfoData MapBinanceCoinInfo(BinanceCoinInfo coin)
        {
            if (coin.Filters == null || coin.Filters.Count < 3)
            {
                Logger.LogDebug($"Filters Null {coin.Symbol}");

                return null;
            }

            var tickSize = coin.Filters[0].TickSize.TrimEnd('0').Length - 2;
            var stepSize = coin.Filters[1].StepSize.TrimEnd('0').Length - 2;
            decimal.TryParse(coin.Filters[2].MinNotional, out var minAmount);

            return new ProxyCoinInfoData
            {
                Symbol = coin.Symbol,
                Market = coin.Market,
                Exchange = Constant.Binance,
                QuantityPrecision = stepSize,
                PricePrecision = tickSize,
                MinimumLimitOrder = minAmount
            };
        }

        public static ProxyCoinTickerData MapBinanceTicker(BinanceTicker ticker)
        {
            return new ProxyCoinTickerData
            {
                Exchange = Constant.Binance,
                Ask = ticker.Price == 0 ? ticker.Ask : ticker.Price,
                Bid = ticker.Price == 0 ? ticker.Bid : ticker.Price,
                Coin = ticker.Coin,
                Last = ticker.Price == 0 ? ticker.Last : ticker.Price,
                Market = ticker.Market,
                TimeStamp = DateTime.Now,
                High = ticker.High,
                Volume = ticker.Volume,
                Low = ticker.Low,
                PrevDayLast = ticker.PrevLastPrice
            };
        }

        public static IList<ProxyCoinOrderData> MapCoinTrade(List<BinanceMyTrade> trades, string symbol)
        {
            var temp = new List<ProxyCoinOrderData>();

            foreach (var trade in trades)
            {
                temp.Add(new ProxyCoinOrderData
                {
                    Symbol = symbol,
                    Market = trade.Symbol.Replace(symbol, string.Empty),
                    Quantity = trade.Quantity,
                    QuantityRemaining = 0,
                    Exchange = Constant.Binance,
                    OrderSide = trade.IsBuyer ? ProxyEnum.CoinOrderSide.BUY : ProxyEnum.CoinOrderSide.SELL,
                    OrderStatus = ProxyEnum.CoinOrderStatus.FILLED,
                    Price = trade.Price,
                    Time = Util.UnixTimestampToDateTime(trade.Time)
                });
            }

            return temp;
        }
    }
}