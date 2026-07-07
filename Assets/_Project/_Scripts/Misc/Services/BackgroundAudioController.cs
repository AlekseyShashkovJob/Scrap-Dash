using UnityEngine;
using System.Collections;

namespace Infrastructure.Platform
{
    public class BackgroundAudioController : MonoBehaviour
    {
        [SerializeField] private AudioClip _track;
        [SerializeField] private float _crossfadeTime = 0.5f;

        private AudioSource _source;
        private Coroutine _activeBlend;

        private static BackgroundAudioController _singleton;

        private void Awake()
        {
            if (_singleton != null && _singleton != this)
            {
                Destroy(gameObject);
                return;
            }

            _singleton = this;
            DontDestroyOnLoad(gameObject);

            _source = gameObject.AddComponent<AudioSource>();
            _source.clip = _track;
            _source.loop = true;
            _source.playOnAwake = false;

            SyncWithPreferences();
        }

        private void OnEnable()
        {
            UI.Screens.MainMenu.PreferencesScreen.OnAudioToggled += SyncWithPreferences;
        }

        private void OnDisable()
        {
            UI.Screens.MainMenu.PreferencesScreen.OnAudioToggled -= SyncWithPreferences;
        }

        private void SyncWithPreferences()
        {
            bool enabled = PlayerPrefs.GetInt(StorageKeys.AudioEnabled, 1) == 1;

            if (_activeBlend != null)
                StopCoroutine(_activeBlend);

            _activeBlend = StartCoroutine(enabled ? RampUp() : RampDown());
        }

        private IEnumerator RampUp()
        {
            if (!_source.isPlaying)
            {
                _source.volume = 0f;
                _source.Play();
            }

            float initial = _source.volume;
            float t = 0f;

            while (t < _crossfadeTime)
            {
                _source.volume = Mathf.Lerp(initial, 1f, t / _crossfadeTime);
                t += Time.unscaledDeltaTime;
                yield return null;
            }

            _source.volume = 1f;
        }

        private IEnumerator RampDown()
        {
            float initial = _source.volume;
            float t = 0f;

            while (t < _crossfadeTime)
            {
                _source.volume = Mathf.Lerp(initial, 0f, t / _crossfadeTime);
                t += Time.unscaledDeltaTime;
                yield return null;
            }

            _source.volume = 0f;
            _source.Stop();
        }
    }
}