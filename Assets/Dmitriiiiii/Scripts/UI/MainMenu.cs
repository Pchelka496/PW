using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string _sceneName;
    
    public void LoadScene()
    {
        if (string.IsNullOrEmpty(_sceneName))
        {
            Debug.LogWarning("Scene name is null or empty!");
            return;
        }

        if (Application.CanStreamedLevelBeLoaded(_sceneName))
        {
            SceneManager.LoadScene(_sceneName);
        }
        else
        {
            Debug.LogError($"Scene '{_sceneName}' не найдена в Build Settings!");
        }
    }
}