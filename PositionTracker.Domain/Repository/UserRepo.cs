using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using PositionTracker.Domain.Entity;
using PositionTracker.Utility;

namespace PositionTracker.Domain.Repository
{
    public class UserRepo
    {
        private static readonly string CombinedPath;
        public static string UserFilePath => "UserData.json";

        static UserRepo()
        {
            CombinedPath = Path.Combine(FileHelper.FilesDir, UserFilePath);
        }

        public static void Load(User user)
        {
            if (!File.Exists(CombinedPath))
            {
                user.UserSettings = new UserSettings();
                user.UserSettings.ApiKeys.Add(Constant.Binance,
                    new ApiKey(Constant.Binance, Constant.BinanceApiKey, Constant.BinanceApiSecret));

                return;
            }

            var userData = File.ReadAllText(CombinedPath);

            User userRead = null;

            try
            {
                userRead = JsonConvert.DeserializeObject<User>(userData);
            }
            catch (Exception e)
            {
                Logger.LogError($"User data corrupted.", e);
            }

            if (userRead == null)
            {
                Logger.LogFatal($"User data is null. Either new user or deleted.");

                return;
            }

            // TODO: better way to make sure apiKeys are updated
            userRead.UserSettings.ApiKeys[Constant.Binance] =
                new ApiKey(Constant.Binance, Constant.BinanceApiKey, Constant.BinanceApiSecret);

            user.UserCoins = userRead.UserCoins;
            user.UserSettings = userRead.UserSettings;
            user.UserSummary = userRead.UserSummary;
            user.CoinsToTrack = userRead.CoinsToTrack;
            user.UserPositions = userRead.UserPositions;
            user.UserWatchlist = userRead.UserWatchlist;
        }

        public static void Save(User user)
        {
            var userData = JsonConvert.SerializeObject(user);

            File.WriteAllText(CombinedPath, userData);
        }
    }
}