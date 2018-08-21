using System.Collections.Generic;

namespace PositionTracker.WebUI.Models
{
    public class SummaryModel
    {
        public IDictionary<string, decimal> Balance { get; set; }
        public IDictionary<string, decimal> RemainingBtc { get; set; }
        public IList<TickerModel> BtcTickers { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal TotalBalanceInUsd { get; set; }
        public string LastTickerTime { get; set; }
        public string LastBtcTickerTime { get; set; }

        public SummaryModel()
        {
            Balance = new Dictionary<string, decimal>();
            RemainingBtc = new Dictionary<string, decimal>();
            BtcTickers = new List<TickerModel>();
        }
    }
}