using System;

namespace PositionTracker.Domain.Entity
{
    public class Enum
    {
        public enum OrderSide
        {
            Buy,
            Sell
        }

        public enum OrderType { }

        [Flags]
        public enum TrackType
        {
            None = 0,
            Position = 1 << 0,
            Watchlist = 1 << 1,
            OrderHistory = 1 << 2
        }
    }
}