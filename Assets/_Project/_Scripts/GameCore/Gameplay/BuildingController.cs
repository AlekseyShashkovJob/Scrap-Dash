using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Gameplay
{
    public class BuildingController : MonoBehaviour
    {
        [Header("Floor Settings")]
        [SerializeField] private Transform _floorContainer;
        [SerializeField] private GameObject _floorPrefab;
        [SerializeField] private Sprite _underConstructionSprite;
        [SerializeField] private Sprite _builtSprite;

        [Header("Positioning")]
        [SerializeField] private float _floorHeight = 150f;
        [SerializeField] private float _startOffsetY = -20f;
        [SerializeField] private float _floorOverlap = 10f;
        [SerializeField] private int _maxVisibleFloors = 5;

        [Header("Workers")]
        [SerializeField] private Transform _workerContainer;
        [SerializeField] private GameObject _workerPrefab;
        [SerializeField] private Sprite[] _workerSprites;
        [SerializeField] private float _underConstructionWorkerOffsetY = 130f;
        [SerializeField] private float _builtWorkerOffsetY = 145f;
        //[SerializeField] private float _workerSpacing = 20f;
        [SerializeField] private float _workerWidth = 40f;
        [SerializeField] private float _floorWidthForWorkers = 280f;

        public event Action<float> OnProgressChanged;
        public event Action<int, int> OnFloorChanged;
        public event Action OnFloorComplete;
        public event Action OnBuildingComplete;

        public int DebrisPerFloor => _debrisPerFloor;

        private int _totalFloors;
        private int _debrisPerFloor;

        private int _currentFloorIndex;
        private int _currentFloorProgress;
        private Image _currentFloorImage;
        private RectTransform _currentFloorRect;

        private int _visibleFloorCount;
        private List<List<GameObject>> _floorWorkers = new List<List<GameObject>>();

        public void Init(int totalFloors, int debrisPerFloor)
        {
            _totalFloors = totalFloors;
            _debrisPerFloor = debrisPerFloor;
            _currentFloorIndex = 0;
            _currentFloorProgress = 0;
            _visibleFloorCount = 0;

            ClearFloors();
            ClearAllWorkers();
            SpawnNextFloor();

            OnProgressChanged?.Invoke(0f);
            OnFloorChanged?.Invoke(1, _totalFloors);
        }

        public void AddProgress()
        {
            _currentFloorProgress++;

            float progress = (float)_currentFloorProgress / _debrisPerFloor;
            OnProgressChanged?.Invoke(progress);

            if (_currentFloorProgress >= _debrisPerFloor)
            {
                CompleteCurrentFloor();
            }
        }

        private void CompleteCurrentFloor()
        {
            _currentFloorImage.sprite = _builtSprite;
            OnFloorComplete?.Invoke();

            RespawnWorkersForFloor(_visibleFloorCount - 1, _builtWorkerOffsetY);

            _currentFloorIndex++;

            if (_currentFloorIndex >= _totalFloors)
            {
                OnFloorChanged?.Invoke(_totalFloors, _totalFloors);
                OnBuildingComplete?.Invoke();
            }
            else
            {
                _currentFloorProgress = 0;
                OnProgressChanged?.Invoke(0f);
                OnFloorChanged?.Invoke(_currentFloorIndex + 1, _totalFloors);
                StartNextFloor();
            }
        }

        private void StartNextFloor()
        {
            if (_visibleFloorCount >= _maxVisibleFloors)
            {
                _currentFloorImage.sprite = _underConstructionSprite;
                RespawnWorkersForFloor(_visibleFloorCount - 1, _underConstructionWorkerOffsetY);
            }
            else
            {
                SpawnNextFloor();
            }
        }

        private void SpawnNextFloor()
        {
            GameObject floor = Instantiate(_floorPrefab, _floorContainer);

            RectTransform rect = floor.GetComponent<RectTransform>();
            float yPos = _startOffsetY + _visibleFloorCount * (_floorHeight - _floorOverlap);
            rect.anchoredPosition = new Vector2(0f, yPos);

            _currentFloorImage = floor.GetComponent<Image>();
            _currentFloorImage.sprite = _underConstructionSprite;
            _currentFloorRect = rect;

            _visibleFloorCount++;

            _floorWorkers.Add(new List<GameObject>());

            SpawnWorkersForFloor(_visibleFloorCount - 1, _underConstructionWorkerOffsetY);
        }

        #region Workers

        private void SpawnWorkersForFloor(int floorVisualIndex, float offsetY)
        {
            int workerCount = UnityEngine.Random.Range(1, 4); // 1-3
            float[] positions = GenerateWorkerPositions(workerCount);

            float floorBaseY = _startOffsetY + floorVisualIndex * (_floorHeight - _floorOverlap);
            float worldY = floorBaseY + offsetY;

            for (int i = 0; i < positions.Length; i++)
            {
                GameObject worker = Instantiate(_workerPrefab, _workerContainer);
                RectTransform rect = worker.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(positions[i], worldY);

                Image img = worker.GetComponent<Image>();
                img.sprite = _workerSprites[UnityEngine.Random.Range(0, _workerSprites.Length)];

                float flipX = UnityEngine.Random.value > 0.5f ? 1f : -1f;
                rect.localScale = new Vector3(flipX, 1f, 1f);

                _floorWorkers[floorVisualIndex].Add(worker);
            }
        }

        private void RespawnWorkersForFloor(int floorVisualIndex, float offsetY)
        {
            ClearWorkersForFloor(floorVisualIndex);
            SpawnWorkersForFloor(floorVisualIndex, offsetY);
        }

        private float[] GenerateWorkerPositions(int count)
        {
            float availableWidth = _floorWidthForWorkers - _workerWidth;
            float halfAvailable = availableWidth * 0.5f;

            float sectionWidth = availableWidth / count;

            float[] positions = new float[count];

            for (int i = 0; i < count; i++)
            {
                float sectionStart = -halfAvailable + i * sectionWidth;
                float sectionEnd = sectionStart + sectionWidth - _workerWidth;

                float pos = UnityEngine.Random.Range(sectionStart, sectionEnd);
                positions[i] = pos + _workerWidth * 0.5f;
            }

            return positions;
        }

        private void ClearWorkersForFloor(int floorVisualIndex)
        {
            if (floorVisualIndex >= _floorWorkers.Count) return;

            foreach (var worker in _floorWorkers[floorVisualIndex])
            {
                if (worker != null)
                    Destroy(worker);
            }

            _floorWorkers[floorVisualIndex].Clear();
        }

        private void ClearAllWorkers()
        {
            foreach (var workerList in _floorWorkers)
            {
                foreach (var worker in workerList)
                {
                    if (worker != null)
                        Destroy(worker);
                }
            }

            _floorWorkers.Clear();
        }

        #endregion

        private void ClearFloors()
        {
            foreach (Transform child in _floorContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }
}