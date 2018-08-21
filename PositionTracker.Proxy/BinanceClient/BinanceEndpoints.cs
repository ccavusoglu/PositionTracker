using PositionTracker.Utility;

namespace PositionTracker.Proxy.BinanceClient
{
    public static class BinanceEndpoints
    {
        public static BinanceEndpointData TestConnectivity => new BinanceEndpointData(Constant.BinanceRestPing);

        public static BinanceEndpointData ServerTime => new BinanceEndpointData(Constant.BinanceRestTime);

        public static BinanceEndpointData ExchangeInfo => new BinanceEndpointData(Constant.BinanceRestExchangeInfo);

        public static BinanceEndpointData TickerAndStats => new BinanceEndpointData(Constant.BinanceRestTickerAndStats);

        public static BinanceEndpointData OpenOrders => new BinanceEndpointData(Constant.BinanceRestOpenOrders, true);

        public static BinanceEndpointData AllOrders => new BinanceEndpointData(Constant.BinanceRestAllOrders, true, 20);

        public static BinanceEndpointData Account => new BinanceEndpointData(Constant.BinanceRestAccount, true, 20);

        public static BinanceEndpointData Depth => new BinanceEndpointData(Constant.BinanceRestDepth);

        public static BinanceEndpointData MyTrades => new BinanceEndpointData(Constant.BinanceRestMyTrades, true, 20);

        public static BinanceEndpointData AggTrades => new BinanceEndpointData(Constant.BinanceRestAggTrades, false, 2);

        public static BinanceEndpointData Trades => new BinanceEndpointData(Constant.BinanceRestTrades);

        public static BinanceEndpointData Klines => new BinanceEndpointData(Constant.BinanceRestKlines, false, 2);

        public static BinanceEndpointData BookTicker => new BinanceEndpointData(Constant.BinanceRestBookTicker);

        public static BinanceEndpointData Price => new BinanceEndpointData(Constant.BinanceRestPrice);

        public static BinanceEndpointData Order => new BinanceEndpointData(Constant.BinanceRestOrder);
    }
}