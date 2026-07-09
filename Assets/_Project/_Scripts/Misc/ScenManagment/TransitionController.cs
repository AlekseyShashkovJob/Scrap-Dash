using UnityEngine;
using UnityEngine.SceneManagement;

namespace Infrastructure.Navigation
{
    public class TransitionController : MonoBehaviour
    {
        public void Navigate(string scene)
        {
            SceneManager.LoadScene(scene);
        }
    }
}