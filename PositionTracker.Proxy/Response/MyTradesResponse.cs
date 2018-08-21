using System.Collections.Generic;
using PositionTracker.Proxy.ProxyEntity;

namespace PositionTracker.Proxy.Response
{
    public class MyTradesResponse : BaseResponse
    {
        public IList<ProxyCoinOrderData> Trades { get; set; }
    }
}