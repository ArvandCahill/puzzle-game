using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void GoToScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private System.Collections.IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            Debug.Log($"Loading Progress: {asyncOperation.progress * 100}%");

            if (asyncOperation.progress >= 0.9f)
            {
                Debug.Log("Loading complete! Activating scene...");
                asyncOperation.allowSceneActivation = true;
            }

            yield return null; 
        }
    }
}
