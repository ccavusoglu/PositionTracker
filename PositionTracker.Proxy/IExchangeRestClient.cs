using System.Threading.Tasks;
using PositionTracker.Proxy.Response;

namespace PositionTracker.Proxy
{
    public interface IExchangeRestClient
    {
        Task<PositionsResponse> FetchPositions(bool onlyDefaultMarket);
        Task<AvailableCoinsResponse> GetExchangeInfo();
        Task<MyTradesResponse> GetMyTrades(string coin, string market);
        Task<CoinsTickerResponse> GetTickers();

        void ThrottleIfNecessary();
    }
}