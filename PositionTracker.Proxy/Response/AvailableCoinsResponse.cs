using System.Collections.Generic;
using PositionTracker.Proxy.ProxyEntity;

namespace PositionTracker.Proxy.Response
{
    public class AvailableCoinsResponse : BaseResponse
    {
        public IList<ProxyCoinInfoData> Coins;
    }
}