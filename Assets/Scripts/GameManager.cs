using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private GameState currentState = GameState.LevelSelector;

    [Header("Sound Effects")]
    public AudioMixer audioMixer;
    private bool isAudioOn = true;
    public AudioSource bgmAudioSource;
    public AudioSource tapAudioSource;
    public AudioClip bgm;
    public AudioClip tap;
    private SoundEffect soundEffect;

    [SerializeField] private Image toggleAudioButton;

    public enum GameState
    {
        LevelSelector,
        Playing
    }

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

        bgmAudioSource = GetComponent<AudioSource>();
        soundEffect = GetComponent<SoundEffect>();
    }

    void Start()
    {
        if (GameObject.FindGameObjectWithTag("SoundButton") != null)
        {
            toggleAudioButton = GameObject.FindGameObjectWithTag("SoundButton").GetComponent<Image>();
        }
        LoadAudioSetting();
        soundEffect.PlayBGM(bgmAudioSource, bgm);

        Application.targetFrameRate = 60;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                soundEffect.PlaySFX(tapAudioSource, tap);
            }
        }
    }

    public void ToggleAudio()
    {
        isAudioOn = !isAudioOn;
        audioMixer.SetFloat("Master", isAudioOn ? 0 : -80);

        if (toggleAudioButton != null)
        {
            toggleAudioButton.color = new Color(toggleAudioButton.color.r, toggleAudioButton.color.g, toggleAudioButton.color.b, isAudioOn ? 1f : 0.5f);
        }

        SaveAudioSetting();
    }

    public bool IsAudioOn()
    {
        return isAudioOn;
    }

    public void SaveAudioSetting()
    {
        PlayerPrefs.SetInt("AudioOn", isAudioOn ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("Audio setting saved: " + (isAudioOn ? "On" : "Off"));
    }

    private void LoadAudioSetting()
    {
        if (PlayerPrefs.HasKey("AudioOn"))
        {
            isAudioOn = PlayerPrefs.GetInt("AudioOn") == 1;
            audioMixer.SetFloat("Master", isAudioOn ? 0 : -80);

            // Update button appearance
            if (toggleAudioButton != null)
            {
                toggleAudioButton.color = new Color(toggleAudioButton.color.r, toggleAudioButton.color.g, toggleAudioButton.color.b, isAudioOn ? 1f : 0.5f);
            }

            Debug.Log("Audio setting loaded: " + (isAudioOn ? "On" : "Off"));
        }
        else
        {
            SaveAudioSetting();
        }
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
