using System.Collections.Generic;
using PositionTracker.Proxy.ProxyEntity;

namespace PositionTracker.Proxy.Response
{
    public class PositionsResponse : BaseResponse
    {
        public IList<ProxyPositionData> Positions { get; set; }
        public Dictionary<string, List<ProxyCoinOrderData>> Trades { get; set; }
    }
}