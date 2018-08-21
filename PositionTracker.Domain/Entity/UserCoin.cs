using System.ComponentModel;
using Newtonsoft.Json;
using PositionTracker.Domain.Repository;

namespace PositionTracker.Domain.Entity
{
    public class UserCoin
    {
        public readonly string Coin;
        public readonly string Exchange;
        public readonly UserCoinKey UniqueName;

        public decimal Balance { get; internal set; }
        public decimal Available { get; internal set; }
        public decimal Pending { get; internal set; }
        public decimal BuyPrice { get; internal set; }
        public decimal Quantity => Available + Pending;

        public string Notes { get; internal set; }

        public UserCoin(string coin, string exchange)
        {
            Coin = coin;
            Exchange = exchange;
            UniqueName = UserCoinKey.Get(Coin, Exchange);
        }

        [JsonConstructor]
        public UserCoin(string coin, string exchange,
            decimal balance, decimal available, decimal pending, decimal buyPrice, string notes) : this(coin, exchange)
        {
            Balance = balance;
            Available = available;
            Pending = pending;
            BuyPrice = buyPrice;
            Notes = notes;
        }

        public override int GetHashCode()
        {
            return UniqueName.GetHashCode();
        }

        public void MergePosition(decimal balance, decimal available, decimal pending, decimal buyPrice)
        {
            Balance = balance;
            Available = available;
            Pending = pending;
            BuyPrice = buyPrice;
        }

        public void SetNotes(string notes)
        {
            Notes = notes;
        }
    }

    [TypeConverter(typeof(UserCoinKeyConverter))]
    public class UserCoinKey
    {
        public readonly string Coin;
        public readonly string Exchange;

        private UserCoinKey(string coin, string exchange)
        {
            Coin = coin;
            Exchange = exchange;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is UserCoinKey rhs)) return false;

            return GetUniqueId(Coin, Exchange).Equals(GetUniqueId(rhs.Coin, rhs.Exchange));
        }

        public static UserCoinKey Get(string symbol, string exchange)
        {
            return new UserCoinKey(symbol, exchange);
        }

        public override int GetHashCode()
        {
            return GetUniqueId(Coin, Exchange).GetHashCode();
        }

        public override string ToString()
        {
            return GetUniqueId(Coin, Exchange);
        }

        internal static string GetUniqueId(string coin, string exchange)
        {
            return $"{coin.ToUpperInvariant()}_{exchange.ToUpperInvariant()}";
        }
    }
}