namespace PositionTracker.Utility
{
    public static class Constant
    {
        public const string Binance = "BINANCE";
        public const string Btc = "BTC";
        public const string BtcSym = "Ƀ";
        public const string TrySym = "₺";
        public const string UsdSym = "$";

        public const string BinanceApiKey = "";
        public const string BinanceApiSecret = "";
        public const string BinanceRestAccount = "v3/account";
        public const string BinanceRestAggTrades = "v1/aggTrades";
        public const string BinanceRestAllOrders = "v3/allOrders";
        public const string BinanceRestBookTicker = "v3/ticker/bookTicker";
        public const string BinanceRestDepth = "v1/depth";
        public const string BinanceRestExchangeInfo = "v1/exchangeInfo";
        public const string BinanceRestKlines = "v1/klines";
        public const string BinanceRestMyTrades = "v3/myTrades";
        public const string BinanceRestOpenOrders = "v3/openOrders";
        public const string BinanceRestOrder = "v3/order";
        public const string BinanceRestPing = "v1/ping";
        public const string BinanceRestPrice = "v3/ticker/price";
        public const string BinanceRestRootUrl = "https://api.binance.com/api/";
        public const string BinanceRestTickerAndStats = "v1/ticker/24hr";
        public const string BinanceRestTime = "v1/time";
        public const string BinanceRestTrades = "v1/trades";
        public const long BinanceRestRequestLimit = 1200;

        public static long RestApiTickerRefreshRate = 10000;
    }
}