using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Infrastructure.Navigation
{
    public class TransitionController : MonoBehaviour
    {
        private readonly float _blendDuration = 0.5f;

        [SerializeField] private GameObject _curtain;

        private CanvasGroup _curtainGroup;
        private bool _inTransition;

        private void Awake()
        {
            _curtainGroup = _curtain.GetComponent<CanvasGroup>();
        }

        public void Navigate(string destination)
        {
            if (!_inTransition)
                StartCoroutine(PerformTransition(destination));
        }

        private IEnumerator PerformTransition(string destination)
        {
            _inTransition = true;
            _curtain.SetActive(true);

            yield return StartCoroutine(BlendAlpha(0f, 1f));

            AsyncOperation op = SceneManager.LoadSceneAsync(destination);
            op.allowSceneActivation = false;

            while (op.progress < 0.9f)
                yield return null;

            yield return new WaitForSeconds(0.5f);
            op.allowSceneActivation = true;

            while (!op.isDone)
                yield return null;

            yield return StartCoroutine(BlendAlpha(1f, 0f));
            _inTransition = false;
        }

        private IEnumerator BlendAlpha(float from, float to)
        {
            float elapsed = 0f;
            while (elapsed <= _blendDuration)
            {
                _curtainGroup.alpha = Mathf.Lerp(from, to, elapsed / _blendDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            _curtainGroup.alpha = to;
        }
    }
}