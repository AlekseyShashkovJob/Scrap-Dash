using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameCore;

namespace View.UI.Game
{
    public class GameHUD : MonoBehaviour
    {
        [Header("Score")]
        [SerializeField] private TMP_Text _scoreText;

        [Header("Lives")]
        [SerializeField] private GameObject[] _hearts;

        [Header("Misses")]
        [SerializeField] private Image[] _missImages;
        [SerializeField] private Sprite _missEmpty;
        [SerializeField] private Sprite _missFilled;

        private void Start()
        {
            GameManager.Instance.OnScoreChanged += UpdateScore;
            GameManager.Instance.OnLivesChanged += UpdateLives;
            GameManager.Instance.OnMissesChanged += UpdateMisses;

            // Инициализируем отображение
            UpdateScore(GameManager.Instance.CurrentScore);
            UpdateLives(GameManager.Instance.Lives);
            UpdateMisses(GameManager.Instance.Misses);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance == null) return;

            GameManager.Instance.OnScoreChanged -= UpdateScore;
            GameManager.Instance.OnLivesChanged -= UpdateLives;
            GameManager.Instance.OnMissesChanged -= UpdateMisses;
        }

        private void UpdateScore(int score)
        {
            _scoreText.text = score.ToString();
        }

        private void UpdateLives(int lives)
        {
            for (int i = 0; i < _hearts.Length; i++)
            {
                _hearts[i].SetActive(i < lives);
            }
        }

        private void UpdateMisses(int misses)
        {
            for (int i = 0; i < _missImages.Length; i++)
            {
                _missImages[i].sprite = i < misses ? _missFilled : _missEmpty;
            }
        }
    }
}