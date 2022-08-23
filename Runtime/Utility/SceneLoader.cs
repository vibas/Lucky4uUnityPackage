using UnityEngine.SceneManagement;

namespace Lucky4u.Utility
{
    public class SceneLoader
    {
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}