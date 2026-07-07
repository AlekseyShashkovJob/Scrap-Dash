using System;
using UnityEngine;
using GameCore.Constants;
using GameCore.Data;
using GameCore.Gameplay;
using UI.Screens.Gameplay;

namespace GameCore
{
    public class SessionDirector : MonoBehaviour
    {
        public static SessionDirector Instance { get; private set; }

        // --- State ---
        public int CurrentPoints { get; private set; }
        public int PersonalRecord { get; private set; }
        public int RemainingLives { get; private set; }
        public int MissCount { get; private set; }
        public int ComboMultiplier { get; private set; }
        public bool IsActive { get; private set; }
        public int ActiveStage { get; private set; }

        // --- Events ---
        public event Action<int> OnPointsUpdated;
        public event Action<int> OnLivesUpdated;
        public event Action<int> OnMissCountUpdated;
        public event Action<int> OnComboUpdated;
        public event Action OnSessionFailed;

        [Header("Stage Profiles")]
        [SerializeField] private LevelConfig[] _stageProfiles;

        [Header("Dependencies")]
        [SerializeField] private Infrastructure.Navigation.TransitionController _navigator;
        [SerializeField] private BuildingController _construction;
        [SerializeField] private DebrisSpawner _dispenser;
        [SerializeField] private FloorProgressIndicator _progressIndicator;
        [SerializeField] private Truck _catcher;

        [Header("Overlays")]
        [SerializeField] private CompletionOverlay _completionOverlay;
        [SerializeField] private DefeatOverlay _defeatOverlay;

        private const int COMBO_CAP = 5;
        private const float IMPEDE_DURATION = 3f;

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

            if (_construction != null)
            {
                _construction.OnProgressChanged -= HandleRatioChanged;
                _construction.OnFloorChanged -= HandleFloorChanged;
                _construction.OnBuildingComplete -= CompleteSession;
            }
        }

        private void Start()
        {
            Time.timeScale = 1f;
            ActiveStage = PlayerPrefs.GetInt(SessionRules.ACTIVE_STAGE_KEY, 0);
            RestoreRecord();
            BeginSession();
        }

        public void BeginSession()
        {
            CurrentPoints = 0;
            RemainingLives = SessionRules.LIFE_POOL;
            MissCount = 0;
            ComboMultiplier = 1;
            IsActive = true;

            LevelConfig profile = _stageProfiles[ActiveStage];

            _construction.OnProgressChanged += HandleRatioChanged;
            _construction.OnFloorChanged += HandleFloorChanged;
            _construction.OnBuildingComplete += CompleteSession;

            _construction.Init(profile.TotalFloors, profile.DebrisPerFloor);
            _dispenser.Init(profile.DebrisFallSpeed, profile.SpawnInterval);

            _progressIndicator.SetProgress(0f);
            _progressIndicator.SetFloorLabel(1, profile.TotalFloors);

            OnPointsUpdated?.Invoke(CurrentPoints);
            OnLivesUpdated?.Invoke(RemainingLives);
            OnMissCountUpdated?.Invoke(MissCount);
            OnComboUpdated?.Invoke(ComboMultiplier);
        }

        public void Terminate()
        {
            IsActive = false;
            _dispenser.Stop();
        }

        #region Points & Combo

        public void AwardPoints(int baseValue)
        {
            if (!IsActive) return;

            CurrentPoints += baseValue * ComboMultiplier;
            OnPointsUpdated?.Invoke(CurrentPoints);

            _construction.AddProgress();
            EscalateCombo();
        }

        private void EscalateCombo()
        {
            if (ComboMultiplier >= COMBO_CAP) return;
            ComboMultiplier++;
            OnComboUpdated?.Invoke(ComboMultiplier);
        }

        private void DropCombo()
        {
            if (ComboMultiplier == 1) return;
            ComboMultiplier = 1;
            OnComboUpdated?.Invoke(ComboMultiplier);
        }

        #endregion

        #region Misses & Lives

        public void CountMiss()
        {
            if (!IsActive) return;

            DropCombo();

            MissCount++;
            OnMissCountUpdated?.Invoke(MissCount);

            if (MissCount >= SessionRules.MISS_THRESHOLD)
            {
                MissCount = 0;
                OnMissCountUpdated?.Invoke(MissCount);
                ConsumeLife();
            }
        }

        private void ConsumeLife()
        {
            RemainingLives--;
            OnLivesUpdated?.Invoke(RemainingLives);

            if (RemainingLives <= 0)
                FailSession();
        }

        #endregion

        #region Hazards & Bonuses

        public void BoostConstruction(float fraction)
        {
            if (!IsActive) return;

            int bonus = Mathf.CeilToInt(_construction.DebrisPerFloor * fraction);

            for (int i = 0; i < bonus; i++)
                _construction.AddProgress();
        }

        public void InflictSlow()
        {
            if (!IsActive) return;

            _catcher.ApplySlow(IMPEDE_DURATION);
            CountMiss();
        }

        public void InflictExplosion()
        {
            if (!IsActive) return;

            RemainingLives--;
            OnLivesUpdated?.Invoke(RemainingLives);

            if (RemainingLives <= 0)
                FailSession();
        }

        #endregion

        #region Progress Callbacks

        private void HandleRatioChanged(float ratio)
        {
            _progressIndicator.SetProgress(ratio);
        }

        private void HandleFloorChanged(int current, int total)
        {
            _progressIndicator.SetFloorLabel(current, total);
        }

        #endregion

        #region Session Results

        public void CompleteSession()
        {
            if (!IsActive) return;
            IsActive = false;

            _dispenser.Stop();
            PersistResults();
            UnlockFollowingStage();

            bool finalStage = ActiveStage >= SessionRules.STAGE_COUNT - 1;
            _completionOverlay.Present(CurrentPoints, PersonalRecord, finalStage,
                RestartSession, AdvanceToNext);
        }

        public void FailSession()
        {
            if (!IsActive) return;
            IsActive = false;

            _dispenser.Stop();
            OnSessionFailed?.Invoke();
            PersistResults();

            _defeatOverlay.Present(CurrentPoints, PersonalRecord);
        }

        #endregion

        #region Progression

        private void UnlockFollowingStage()
        {
            int progress = PlayerPrefs.GetInt(SessionRules.PROGRESS_STAGE_KEY, 0);
            if (ActiveStage >= progress && ActiveStage < SessionRules.STAGE_COUNT - 1)
            {
                PlayerPrefs.SetInt(SessionRules.PROGRESS_STAGE_KEY, ActiveStage + 1);
                PlayerPrefs.Save();
            }
        }

        private void AdvanceToNext()
        {
            int next = ActiveStage + 1;
            PlayerPrefs.SetInt(SessionRules.ACTIVE_STAGE_KEY, next);
            PlayerPrefs.Save();
            Time.timeScale = 1f;
            _navigator.Navigate(Infrastructure.Configuration.NavigationRoutes.SESSION_SCENE);
        }

        private void RestartSession()
        {
            Time.timeScale = 1f;
            _navigator.Navigate(Infrastructure.Configuration.NavigationRoutes.SESSION_SCENE);
        }

        #endregion

        #region Persistence

        private void PersistResults()
        {
            if (CurrentPoints > PersonalRecord)
                PersonalRecord = CurrentPoints;
            CommitRecord();
        }

        private void CommitRecord()
        {
            PlayerPrefs.SetInt(SessionRules.RecordKey(ActiveStage), PersonalRecord);
            PlayerPrefs.Save();
        }

        private void RestoreRecord()
        {
            PersonalRecord = PlayerPrefs.GetInt(SessionRules.RecordKey(ActiveStage), 0);
        }

        #endregion
    }
}