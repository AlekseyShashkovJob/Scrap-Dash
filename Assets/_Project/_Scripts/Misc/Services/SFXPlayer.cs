using UnityEngine;

namespace Infrastructure.Platform
{
    public class SFXPlayer : MonoBehaviour
    {
        public static SFXPlayer Instance { get; private set; }

        private AudioSource _source;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _source = gameObject.AddComponent<AudioSource>();
            _source.playOnAwake = false;
            _source.spatialBlend = 0f;
        }

        public void PlayOneShot(AudioClip clip)
        {
            if (clip != null)
                _source.PlayOneShot(clip);
        }
    }
}