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

        public enum GameState
        {
            LevelSelector,
            Playing
        }

        private GameState currentState = GameState.LevelSelector; 

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

        public void SetGameStateToLevelSelector()
        {
            currentState = GameState.LevelSelector;
            Debug.Log("Game State set to LevelSelector.");
        }

        public void SetGameStateToPlaying()
        {
            currentState = GameState.Playing;
            Debug.Log("Game State set to Playing.");
        }

        public string GetGameState()
        {
            return currentState.ToString();
        }
    }
