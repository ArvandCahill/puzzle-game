using UnityEngine;
using UnityEngine.UI;

public class AudioToggleButton : MonoBehaviour
{
    [SerializeField] private Image buttonImage;

    private void Start()
    {
        LoadButtonAppearance();
    }

    public void ToggleAudio()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ToggleAudio();
            UpdateButtonAppearance(GameManager.Instance.IsAudioOn());
        }
    }

    public void LoadButtonAppearance()
    {
        if (GameManager.Instance != null)
        {
            bool isAudioOn = GameManager.Instance.IsAudioOn();
            UpdateButtonAppearance(isAudioOn); 
        }
        else
        {
            Debug.LogWarning("GameManager.Instance is null. Make sure GameManager is initialized.");
        }
    }

    private void UpdateButtonAppearance(bool isAudioOn)
    {
        if (buttonImage != null)
        {
            buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, isAudioOn ? 1f : 0.5f);
        }
        else
        {
            Debug.LogWarning("ButtonImage is not assigned. Make sure it is set in the Inspector.");
        }
    }
}
