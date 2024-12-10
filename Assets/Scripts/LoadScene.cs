using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void Load(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
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
