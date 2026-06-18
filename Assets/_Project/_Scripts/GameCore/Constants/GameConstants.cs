namespace GameCore.Constants
{
    public static class GameConstants
    {
        public const string LAST_UNLOCKED_LEVEL_KEY = "LastUnlockedLevel";
        public const string LAST_SELECTED_LEVEL_KEY = "LastSelectedLevel";

        public const int TOTAL_LEVELS = 12;
        public const int MAX_LIVES = 3;
        public const int MAX_MISSES = 5;

        public static string GetBestScoreKey(int levelIndex)
        {
            return $"BestScore_Level_{levelIndex}";
        }
    }
}