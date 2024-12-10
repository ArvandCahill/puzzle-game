using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    private GameManager gameManager;
    private ProceduralPuzzle proceduralPuzzle;

    private void Start()
    {
        gameManager = GameManager.Instance;
        proceduralPuzzle = FindObjectOfType<ProceduralPuzzle>();

        if (gameManager == null)
        {
            Debug.LogError("GameManager not found. Make sure GameManager is added to the scene.");
        }

        if (proceduralPuzzle == null)
        {
            Debug.LogError("ProceduralPuzzle not found. Make sure ProceduralPuzzle script is added to the game.");
        }
    }

    public void OnBackButtonPressed()
    {
        if (gameManager == null) return;

        string currentState = gameManager.GetGameState();

        if (currentState == "LevelSelector")
        {
            StartCoroutine(LoadMainMenuScene());
        }
        else if (currentState == "Playing")
        {
            if (proceduralPuzzle != null)
            {
                proceduralPuzzle.RestartGame();
            }
            else
            {
                Debug.LogError("ProceduralPuzzle instance not found.");
            }
        }
        else
        {
            Debug.LogWarning("Unrecognized GameState: " + currentState);
        }
    }

    private IEnumerator LoadMainMenuScene()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("MainMenu");
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
