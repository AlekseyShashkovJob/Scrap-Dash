namespace ScrapDash.GameCore.Constants
{
    public static class SessionRules
    {
        public const string PROGRESS_STAGE_KEY = "ProgressStage";
        public const string ACTIVE_STAGE_KEY = "ActiveStage";

        public const int STAGE_COUNT = 12;
        public const int LIFE_POOL = 3;
        public const int MISS_THRESHOLD = 5;

        public static string RecordKey(int stageIndex)
        {
            return $"HighScore_S_{stageIndex}";
        }
    }
}