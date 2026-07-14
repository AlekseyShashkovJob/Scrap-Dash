using UnityEngine;

namespace ScrapDash.GameCore.Data
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Game/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        [SerializeField] private int _totalFloors = 1;
        [SerializeField] private int _debrisPerFloor = 10;
        [SerializeField] private float _debrisFallSpeed = 400f;
        [SerializeField] private float _spawnInterval = 1.5f;

        public int TotalFloors => _totalFloors;
        public int DebrisPerFloor => _debrisPerFloor;
        public float DebrisFallSpeed => _debrisFallSpeed;
        public float SpawnInterval => _spawnInterval;
    }
}