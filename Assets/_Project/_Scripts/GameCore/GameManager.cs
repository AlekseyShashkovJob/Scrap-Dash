using System;
using UnityEngine;
using GameCore.Constants;
using GameCore.Data;
using GameCore.Gameplay;
using View.UI.Game;

namespace GameCore
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        // --- State ---
        public int CurrentScore { get; private set; }
        public int BestScore { get; private set; }
        public int Lives { get; private set; }
        public int Misses { get; private set; }
        public int Multiplier { get; private set; }
        public bool IsGameActive { get; private set; }
        public int CurrentLevel { get; private set; }

        // --- Events ---
        public event Action<int> OnScoreChanged;
        public event Action<int> OnLivesChanged;
        public event Action<int> OnMissesChanged;
        public event Action<int> OnMultiplierChanged;
        public event Action OnGameOver;

        [Header("Level Configs")]
        [SerializeField] private LevelConfig[] _levelConfigs;

        [Header("References")]
        [SerializeField] private Misc.SceneManagment.SceneLoader _sceneLoader;
        [SerializeField] private BuildingController _buildingController;
        [SerializeField] private DebrisSpawner _debrisSpawner;
        [SerializeField] private ProgressBarUI _progressBar;
        [SerializeField] private Truck _truck;

        [Header("Screens")]
        [SerializeField] private VictoryScreen _winScreen;
        [SerializeField] private LoseScreen _loseScreen;

        private const int MAX_MULTIPLIER = 5;
        private const float BARREL_SLOW_DURATION = 3f;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;

            if (_buildingController != null)
            {
                _buildingController.OnProgressChanged -= HandleProgressChanged;
                _buildingController.OnFloorChanged -= HandleFloorChanged;
                _buildingController.OnBuildingComplete -= WinGame;
            }
        }

        private void Start()
        {
            Time.timeScale = 1f;
            CurrentLevel = PlayerPrefs.GetInt(GameConstants.LAST_SELECTED_LEVEL_KEY, 0);
            LoadData();
            StartGame();
        }

        public void StartGame()
        {
            CurrentScore = 0;
            Lives = GameConstants.MAX_LIVES;
            Misses = 0;
            Multiplier = 1;
            IsGameActive = true;

            LevelConfig config = _levelConfigs[CurrentLevel];

            // Подписки
            _buildingController.OnProgressChanged += HandleProgressChanged;
            _buildingController.OnFloorChanged += HandleFloorChanged;
            _buildingController.OnBuildingComplete += WinGame;

            // Инициализация
            _buildingController.Init(config.TotalFloors, config.DebrisPerFloor);
            _debrisSpawner.Init(config.DebrisFallSpeed, config.SpawnInterval);

            // Явный сброс UI
            _progressBar.UpdateProgress(0f);
            _progressBar.UpdateFloor(1, config.TotalFloors);

            // Обновляем счётчики
            OnScoreChanged?.Invoke(CurrentScore);
            OnLivesChanged?.Invoke(Lives);
            OnMissesChanged?.Invoke(Misses);
            OnMultiplierChanged?.Invoke(Multiplier);
        }

        public void StopGame()
        {
            IsGameActive = false;
            _debrisSpawner.Stop();
        }

        #region Score & Multiplier

        public void AddScore(int points)
        {
            if (!IsGameActive) return;

            CurrentScore += points * Multiplier;
            OnScoreChanged?.Invoke(CurrentScore);

            _buildingController.AddProgress();
            IncreaseMultiplier();
        }

        private void IncreaseMultiplier()
        {
            if (Multiplier >= MAX_MULTIPLIER) return;
            Multiplier++;
            OnMultiplierChanged?.Invoke(Multiplier);
        }

        private void ResetMultiplier()
        {
            if (Multiplier == 1) return;
            Multiplier = 1;
            OnMultiplierChanged?.Invoke(Multiplier);
        }

        #endregion

        #region Misses & Lives

        public void RegisterMiss()
        {
            if (!IsGameActive) return;

            ResetMultiplier();

            Misses++;
            OnMissesChanged?.Invoke(Misses);

            if (Misses >= GameConstants.MAX_MISSES)
            {
                Misses = 0;
                OnMissesChanged?.Invoke(Misses);
                LoseLife();
            }
        }

        private void LoseLife()
        {
            Lives--;
            OnLivesChanged?.Invoke(Lives);

            if (Lives <= 0)
                LoseGame();
        }

        #endregion

        #region Bonuses & Traps

        public void AddBuildingProgress(float bonusProgress)
        {
            if (!IsGameActive) return;

            int bonusDebris = Mathf.CeilToInt(_buildingController.DebrisPerFloor * bonusProgress);

            for (int i = 0; i < bonusDebris; i++)
            {
                _buildingController.AddProgress();
            }
        }

        public void ApplySlowTrap()
        {
            if (!IsGameActive) return;

            _truck.ApplySlow(BARREL_SLOW_DURATION);
            RegisterMiss();
        }

        public void ApplyBombTrap()
        {
            if (!IsGameActive) return;

            Lives--;
            OnLivesChanged?.Invoke(Lives);

            if (Lives <= 0)
                LoseGame();
        }

        #endregion

        #region Building Progress

        private void HandleProgressChanged(float progress)
        {
            _progressBar.UpdateProgress(progress);
        }

        private void HandleFloorChanged(int current, int total)
        {
            _progressBar.UpdateFloor(current, total);
        }

        #endregion

        #region Win / Lose

        public void WinGame()
        {
            if (!IsGameActive) return;
            IsGameActive = false;

            _debrisSpawner.Stop();
            SaveSession();
            UnlockNextLevel();

            bool isLastLevel = CurrentLevel >= GameConstants.TOTAL_LEVELS - 1;
            _winScreen.Show(CurrentScore, BestScore, isLastLevel, RestartGame, LoadNextLevel);
        }

        public void LoseGame()
        {
            if (!IsGameActive) return;
            IsGameActive = false;

            _debrisSpawner.Stop();
            OnGameOver?.Invoke();
            SaveSession();

            _loseScreen.Show(CurrentScore, BestScore);
        }

        #endregion

        #region Level Management

        private void UnlockNextLevel()
        {
            int unlocked = PlayerPrefs.GetInt(GameConstants.LAST_UNLOCKED_LEVEL_KEY, 0);
            if (CurrentLevel >= unlocked && CurrentLevel < GameConstants.TOTAL_LEVELS - 1)
            {
                PlayerPrefs.SetInt(GameConstants.LAST_UNLOCKED_LEVEL_KEY, CurrentLevel + 1);
                PlayerPrefs.Save();
            }
        }

        private void LoadNextLevel()
        {
            int nextLevel = CurrentLevel + 1;
            PlayerPrefs.SetInt(GameConstants.LAST_SELECTED_LEVEL_KEY, nextLevel);
            PlayerPrefs.Save();
            Time.timeScale = 1f;
            _sceneLoader.ChangeScene(Misc.Data.SceneConstants.GAME_SCENE);
        }

        private void RestartGame()
        {
            Time.timeScale = 1f;
            _sceneLoader.ChangeScene(Misc.Data.SceneConstants.GAME_SCENE);
        }

        #endregion

        #region Save / Load

        private void SaveSession()
        {
            if (CurrentScore > BestScore)
                BestScore = CurrentScore;
            SaveData();
        }

        private void SaveData()
        {
            PlayerPrefs.SetInt(GameConstants.GetBestScoreKey(CurrentLevel), BestScore);
            PlayerPrefs.Save();
        }

        private void LoadData()
        {
            BestScore = PlayerPrefs.GetInt(GameConstants.GetBestScoreKey(CurrentLevel), 0);
        }

        #endregion
    }
}