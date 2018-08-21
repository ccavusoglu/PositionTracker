using System.Collections.Generic;
using PositionTracker.Proxy.ProxyEntity;

namespace PositionTracker.Proxy.Response
{
    public class CoinsTickerResponse : BaseResponse
    {
        public List<ProxyCoinTickerData> Tickers { get; set; }
    }
}