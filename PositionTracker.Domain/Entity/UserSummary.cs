using System.Collections.Generic;

namespace PositionTracker.Domain.Entity
{
    public class UserSummary
    {
        public IDictionary<string, decimal> Balance { get; internal set; }
        public IDictionary<string, decimal> RemainingBtc { get; internal set; }
        public IDictionary<string, CoinMarketTicker> BtcTickers { get; internal set; }
        public decimal TotalBalance { get; internal set; }

        public UserSummary()
        {
            Balance = new Dictionary<string, decimal>();
            RemainingBtc = new Dictionary<string, decimal>();
            BtcTickers = new Dictionary<string, CoinMarketTicker>();
        }

        public void SetBalance(string exchange, decimal total)
        {
            Balance[exchange] = total;
        }

        public void SetRemainingBtc(string exchange, decimal total)
        {
            Balance[exchange] = total;
        }

        public void SetTotalBalance(decimal total)
        {
            TotalBalance = total;
        }
    }
}