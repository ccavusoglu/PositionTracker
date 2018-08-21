using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using PositionTracker.Proxy.Response;

namespace PositionTracker.Proxy
{
    public abstract class BaseRestClient
    {
        protected volatile bool ApiAlive;
        protected string ApiKey;
        protected string ApiSecret;
        protected HttpClient HttpClient;
        protected bool IsApiSigned;
        protected DateTime LastSuccessfulResponseTime;
        protected object LockObject = new object();
        protected bool PreventThrottle;
        protected volatile bool RequestLimitExceeded;
        protected DateTime RequestLimitIntervalStartTime;
        protected long RequestMinRemainingMs;
        protected ManualResetEvent RequestThrottleResetEvent;
        protected IDictionary<Type, BaseResponse> ResponseCache;
        protected TimeSpan TimestampOffset;
        protected abstract string Name { get; set; }
        protected abstract int RequestLimitExceededWaitTimeMs { get; set; }
        protected abstract long RequestLimitPerMinute { get; set; }

        protected BaseRestClient()
        {
            RequestThrottleResetEvent = new ManualResetEvent(true);
            RequestLimitIntervalStartTime = DateTime.Now;
            ResponseCache = new Dictionary<Type, BaseResponse>();
            ApiAlive = true;
            TimestampOffset = TimeSpan.Zero;
        }

        public abstract void SetApiKey(string key, string secret);
    }
}