using UnityEngine;

namespace GameCore.Gameplay
{
    public class DebrisSpawner : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject[] _trashPrefabs;
        [SerializeField] private GameObject[] _bonusPrefabs;  // [0] Helmet, [1] Drawing
        [SerializeField] private GameObject[] _trapPrefabs;   // [0] Barrel, [1] Bomb

        [Header("Spawn Settings")]
        [SerializeField] private RectTransform _container;
        [SerializeField][Range(0f, 1f)] private float _specialChance = 0.15f;

        private float _spawnInterval;
        private float _fallSpeed;
        private float _spawnTimer;
        private bool _isActive;

        public void Init(float fallSpeed, float spawnInterval)
        {
            _fallSpeed = fallSpeed;
            _spawnInterval = spawnInterval;
            _spawnTimer = 0f;
            _isActive = true;
        }

        public void Stop()
        {
            _isActive = false;
            ClearAllDebris();
        }

        private void Update()
        {
            if (!_isActive) return;

            _spawnTimer += Time.deltaTime;
            if (_spawnTimer >= _spawnInterval)
            {
                _spawnTimer = 0f;
                Spawn();
            }
        }

        private void ClearAllDebris()
        {
            foreach (Transform child in _container)
            {
                Destroy(child.gameObject);
            }
        }

        private void Spawn()
        {
            GameObject prefab;
            DebrisType type;

            float roll = Random.value;

            if (roll < _specialChance)
            {
                if (Random.value < 0.5f)
                {
                    int index = Random.Range(0, _bonusPrefabs.Length);
                    prefab = _bonusPrefabs[index];
                    type = index == 0 ? DebrisType.Helmet : DebrisType.Drawing;
                }
                else
                {
                    int index = Random.Range(0, _trapPrefabs.Length);
                    prefab = _trapPrefabs[index];
                    type = index == 0 ? DebrisType.Barrel : DebrisType.Bomb;
                }
            }
            else
            {
                prefab = _trashPrefabs[Random.Range(0, _trashPrefabs.Length)];
                type = DebrisType.Trash;
            }

            GameObject obj = Instantiate(prefab, _container);

            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = GetSpawnPosition();
            rect.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 180f));

            Debris debris = obj.GetComponent<Debris>();
            debris.Init(type, _fallSpeed, OnDebrisCaught, OnDebrisMissed);
        }

        private Vector2 GetSpawnPosition()
        {
            float halfWidth = _container.rect.width * 0.5f;
            float topY = _container.rect.height * 0.5f;
            float randomX = Random.Range(-halfWidth, halfWidth);

            return new Vector2(randomX, topY);
        }

        private void OnDebrisCaught(Debris debris)
        {
            Infrastructure.Platform.HapticFeedback.Trigger();

            switch (debris.Type)
            {
                case DebrisType.Trash:
                    SessionDirector.Instance.AwardPoints(1);
                    break;

                case DebrisType.Helmet:
                    SessionDirector.Instance.AwardPoints(5);
                    break;

                case DebrisType.Drawing:
                    SessionDirector.Instance.AwardPoints(1);
                    SessionDirector.Instance.BoostConstruction(0.2f);
                    break;

                case DebrisType.Barrel:
                    SessionDirector.Instance.InflictSlow();
                    break;

                case DebrisType.Bomb:
                    SessionDirector.Instance.InflictExplosion();
                    break;
            }
        }

        private void OnDebrisMissed(Debris debris)
        {
            if (debris.Type == DebrisType.Barrel || debris.Type == DebrisType.Bomb)
                return;

            SessionDirector.Instance.CountMiss();
        }
    }
}