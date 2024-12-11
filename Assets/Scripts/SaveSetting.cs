using UnityEngine;

public class SaveSetting : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.HasKey("AudioOn"))
        {
            bool isAudioOn = PlayerPrefs.GetInt("AudioOn") == 1; 
            GameManager.Instance.ToggleAudio();
            if (GameManager.Instance.IsAudioOn() != isAudioOn)
            {
                GameManager.Instance.ToggleAudio();
            }
        }
        else
        {
            SaveAudioSetting(GameManager.Instance.IsAudioOn());
        }
    }

    public void SaveAudioSetting(bool isAudioOn)
    {
        PlayerPrefs.SetInt("AudioOn", isAudioOn ? 1 : 0);
        PlayerPrefs.Save(); 
        Debug.Log("Audio setting saved: " + (isAudioOn ? "On" : "Off"));
    }
}
