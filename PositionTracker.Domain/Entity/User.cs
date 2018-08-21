using System.Collections.Concurrent;
using System.Collections.Generic;
using Newtonsoft.Json;
using PositionTracker.Domain.Repository;
using PositionTracker.Utility;

namespace PositionTracker.Domain.Entity
{
    public class User
    {
        public UserSettings UserSettings { get; set; }
        public ConcurrentDictionary<UserCoinKey, UserCoin> UserCoins { get; internal set; }

        public ConcurrentDictionary<string, List<UserCoin>> UserPositions { get; internal set; }
        public ConcurrentDictionary<string, List<UserCoin>> UserWatchlist { get; internal set; }

        [JsonIgnore]
        public Dictionary<string, ConcurrentDictionary<string, List<UserTrade>>> UserTrades { get; internal set; }

        public ConcurrentDictionary<UserCoin, Enum.TrackType> CoinsToTrack { get; internal set; }

        public UserSummary UserSummary { get; set; }

        public User()
        {
            UserCoins = new ConcurrentDictionary<UserCoinKey, UserCoin>();
            UserPositions = new ConcurrentDictionary<string, List<UserCoin>>();
            UserWatchlist = new ConcurrentDictionary<string, List<UserCoin>>();
            UserTrades = new Dictionary<string, ConcurrentDictionary<string, List<UserTrade>>>();
            CoinsToTrack = new ConcurrentDictionary<UserCoin, Enum.TrackType>();
        }

        public UserCoin GetCoin(string coin, string exchange)
        {
            var userCoinKey = UserCoinKey.Get(coin, exchange);

            return UserCoins.ContainsKey(userCoinKey) ? UserCoins[userCoinKey] : null;
        }

        public void Load()
        {
            UserRepo.Load(this);
        }

        public void Save()
        {
            UserRepo.Save(this);
        }
    }
}