using System.Collections.Generic;
using PositionTracker.Core;
using PositionTracker.Event;
using PositionTracker.Event.Events;
using PositionTracker.Utility;
using PositionTracker.WebUI.Hub;
using PositionTracker.WebUI.Models;

namespace PositionTracker.WebUI.Controllers
{
    public class PositionController
    {
        private readonly UserManager userManager;

        public PositionController(UserManager userManager)
        {
            this.userManager = userManager;

            EventManager.Instance.Subscribe<FetchPositionsEvent>(GetPositions);
            EventManager.Instance.Subscribe<GetTickersEvent>(GetPositions);
            EventManager.Instance.Subscribe<GetSummaryEvent>(GetSummaryData);
        }

        public void EditNotes(string coin, string exchange, string notes)
        {
            userManager.SetNotes(coin, exchange, notes);
        }

        public void FetchPositions()
        {
            userManager.FetchPositions();
        }

        public void GetPositions(IEventBase obj)
        {
            var positions = userManager.GetPositions();

            var temp = new List<PositionGridModel>();

            foreach (var position in positions)
            {
                var ticker = userManager.GetTicker(position.Coin, position.Exchange);

                temp.Add(new PositionGridModel
                {
                    Coin = position.Coin,
                    UniqueName = position.UniqueName.ToString(),
                    Balance = position.Quantity * ticker?.Last ?? 0,
                    Exchange = position.Exchange,
                    LastPrice = ticker?.Last ?? 0,
                    TickerSymbol = ticker?.TickerSymbol,
                    Quantity = position.Quantity,
                    Volume = ticker?.Volume ?? 0,
                    Price = position.BuyPrice,
                    Notes = position.Notes,
                    Change = ticker?.PriceChange24Hr ?? 0,
                    ProfitPercentage = userManager.GetProfitPercentage(ticker, position),
                    TotalPercentage = userManager.GetTotalPercentage(ticker, position)
                });
            }

            temp.Sort((lhs, rhs) => rhs.TotalPercentage.CompareTo(lhs.TotalPercentage));

            MainController.MainHub.SendMessage("position", temp);
        }

        public void Save()
        {
            userManager.SaveUserData();
        }

        private void GetSummaryData(IEventBase obj)
        {
            var userSummary = userManager.GetSummary();

            var btcTickers = new List<TickerModel>();

            foreach (var btcTicker in userSummary.BtcTickers)
            {
                btcTickers.Add(new TickerModel
                {
                    Coin = btcTicker.Value.Coin,
                    Exchange = btcTicker.Value.Exchange,
                    Ticker = btcTicker.Value.Last,
                    TickerSymbol = Util.GetCurrencySymbol(btcTicker.Value.TickerSymbol)
                });
            }

            var userSummaryModel = new SummaryModel
            {
                TotalBalance = userSummary.TotalBalance,
                Balance = userSummary.Balance,
                RemainingBtc = userSummary.RemainingBtc,
                BtcTickers = btcTickers
            };

            MainController.MainHub.SendMessage("summary", userSummaryModel);
        }
    }
}