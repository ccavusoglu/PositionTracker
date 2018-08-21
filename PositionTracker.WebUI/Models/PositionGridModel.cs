using System;

namespace PositionTracker.WebUI.Models
{
    public class PositionGridModel : IComparable
    {
        public static int ChangeIndex = 7;
        public static int NotesIndex = 10;
        public static int VolumeDiffIndex = 9;
        public static int VolumeIndex = 6;
        public static int WallIndex = 8;

        public string UniqueName { get; set; }

        public string Coin { get; set; }

        public string Exchange { get; set; }

        public string TickerSymbol { get; set; }

        public decimal Quantity { get; set; }

        public decimal Balance { get; set; }

        public decimal TotalPercentage { get; set; }

        public decimal Price { get; set; }

        public decimal LastPrice { get; set; }

        public decimal Profit => (LastPrice - Price) * Quantity;

        public decimal ProfitPercentage { get; set; }

        public decimal Change { get; set; }

        public decimal Volume { get; set; }

        public bool IsVolumeIncreased { get; set; }

        public string QuantityStr => $"{Quantity:F8}";

        public virtual string AmountStr => $"{TickerSymbol}{Balance:F8} {TotalPercentage:F2}%";

        public virtual string PriceStr => $"{TickerSymbol}{Price:F8}";

        public virtual string LastPriceStr => $"{TickerSymbol}{LastPrice:F8}";

        public virtual string ProfitStr => $"{TickerSymbol}{Profit:F8} {ProfitPercentage:F2}%";

        public virtual string VolumeStr => $"{TickerSymbol}{Volume:F2}";

        public virtual string ChangeStr => $"{Change:F2}%";

        public virtual string Notes { get; set; }

        public int CompareTo(object obj)
        {
            if (!(obj is PositionGridModel))
                return 1;

            if (Balance > ((PositionGridModel) obj).Balance) return -1;
            if (Balance < ((PositionGridModel) obj).Balance) return 1;

            return 0;
        }
    }
}