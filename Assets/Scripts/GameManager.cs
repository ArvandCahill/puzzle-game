using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 

    [Header("Audio Settings")]
    public AudioMixer audioMixer;
    private bool isAudioOn = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
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

    public void ToggleAudio()
    {
        isAudioOn = !isAudioOn;

        if (isAudioOn)
        {
            audioMixer.SetFloat("MasterVolume", 0); 
            Debug.Log("Audio turned ON");
        }
        else
        {
            audioMixer.SetFloat("MasterVolume", -80); 
            Debug.Log("Audio turned OFF");
        }
    }

    public bool IsAudioOn()
    {
        return isAudioOn;
    }
}
