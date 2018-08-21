using System;

namespace PositionTracker.Proxy.Response
{
    public class BaseResponse
    {
        public string ErrorCode;
        public string ErrorMessage;
        public string Exchange;
        public bool IsCached;
        public bool IsSuccess;
        public bool Retry;
        public DateTime TimeStamp;
    }
}