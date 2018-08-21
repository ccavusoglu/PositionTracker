using PositionTracker.Domain.Entity;
using PositionTracker.Proxy.ProxyEntity;
using PositionTracker.Utility;

namespace PositionTracker.Core
{
    public class DomainMapper
    {
        public static Coin MapCoin(ProxyCoinInfoData coinInfo)
        {
            var exchangeSettings = new CoinExchangeSettings(coinInfo.Exchange, coinInfo.MinimumLimitOrder,
                coinInfo.PricePrecision, coinInfo.QuantityPrecision);

            return new Coin(coinInfo.Symbol, coinInfo.Symbol, coinInfo.Exchange, coinInfo.Market, exchangeSettings);
        }

        public static CoinTicker MapCoinTicker(ProxyCoinTickerData ticker)
        {
            return new CoinTicker(ticker.Coin, ticker.Exchange, new CoinMarketTicker(
                ticker.Coin, ticker.Exchange, ticker.Market, Util.GetCurrencySymbol(ticker.Market),
                ticker.Ask, ticker.Bid, ticker.Last, ticker.High,
                ticker.Low, ticker.PrevDayLast, ticker.Volume, ticker.TimeStamp));
        }

        public static Enum.OrderSide MapOrderSide(ProxyEnum.CoinOrderSide side)
        {
            return side == ProxyEnum.CoinOrderSide.BUY ? Enum.OrderSide.Buy : Enum.OrderSide.Sell;
        }

        public static UserTrade MapTrade(ProxyCoinOrderData order)
        {
            return new UserTrade(order.Symbol, order.Exchange, order.QuantityBalance,
                order.Price, MapOrderSide(order.OrderSide));
        }
    }
}