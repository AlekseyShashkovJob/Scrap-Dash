using GameCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.Gameplay
{
    public class SessionStatusPanel : MonoBehaviour
    {
        [Header("Score")]
        [SerializeField] private TMP_Text _pointsLabel;

        [Header("Lives")]
        [SerializeField] private GameObject[] _lifeIcons;

        [Header("Misses")]
        [SerializeField] private Image[] _missSlots;
        [SerializeField] private Sprite _slotEmpty;
        [SerializeField] private Sprite _slotFilled;

        private void Start()
        {
            SessionDirector.Instance.OnPointsUpdated += RefreshPoints;
            SessionDirector.Instance.OnLivesUpdated += RefreshLives;
            SessionDirector.Instance.OnMissCountUpdated += RefreshMisses;

            RefreshPoints(SessionDirector.Instance.CurrentPoints);
            RefreshLives(SessionDirector.Instance.RemainingLives);
            RefreshMisses(SessionDirector.Instance.MissCount);
        }

        private void OnDestroy()
        {
            if (SessionDirector.Instance == null) return;

            SessionDirector.Instance.OnPointsUpdated -= RefreshPoints;
            SessionDirector.Instance.OnLivesUpdated -= RefreshLives;
            SessionDirector.Instance.OnMissCountUpdated -= RefreshMisses;
        }

        private void RefreshPoints(int value)
        {
            _pointsLabel.text = value.ToString();
        }

        private void RefreshLives(int count)
        {
            for (int i = 0; i < _lifeIcons.Length; i++)
                _lifeIcons[i].SetActive(i < count);
        }

        private void RefreshMisses(int count)
        {
            for (int i = 0; i < _missSlots.Length; i++)
                _missSlots[i].sprite = i < count ? _slotFilled : _slotEmpty;
        }
    }
}