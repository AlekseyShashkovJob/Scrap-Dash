using UnityEngine;

namespace Infrastructure.Platform
{
    public class AudioSystem : MonoBehaviour
    {
        public static AudioSystem Instance { get; private set; }

        [SerializeField] private AudioClip _backgroundTrack;
        [SerializeField] private float _musicVolume = 0.8f;

        private AudioSource _musicSource;
        private AudioSource _effectsSource;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.clip = _backgroundTrack;
            _musicSource.loop = true;
            _musicSource.playOnAwake = false;
            _musicSource.volume = _musicVolume;

            _effectsSource = gameObject.AddComponent<AudioSource>();
            _effectsSource.playOnAwake = false;
            _effectsSource.spatialBlend = 0f;

            ApplyAudioState();
        }

        private void OnEnable()
        {
            UI.Screens.MainMenu.PreferencesScreen.OnAudioToggled += ApplyAudioState;
        }

        private void OnDisable()
        {
            UI.Screens.MainMenu.PreferencesScreen.OnAudioToggled -= ApplyAudioState;
        }

        public void PlayEffect(AudioClip clip)
        {
            if (clip == null) return;
            if (!IsAudioOn()) return;

            _effectsSource.PlayOneShot(clip);
        }

        private void ApplyAudioState()
        {
            bool active = IsAudioOn();

            if (active)
            {
                if (!_musicSource.isPlaying)
                    _musicSource.Play();

                _musicSource.volume = _musicVolume;
            }
            else
            {
                _musicSource.Stop();
                _musicSource.volume = 0f;
            }
        }

        private bool IsAudioOn()
        {
            return PlayerPrefs.GetInt(StorageKeys.AudioEnabled, 1) == 1;
        }
    }
}