using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundEffect : MonoBehaviour
{

    public void PlayBGM(AudioSource audioSource, AudioClip bgm)
    {
        if (audioSource != null && bgm != null)
        {
            audioSource.clip = bgm;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void PlaySFX(AudioSource audioSource, AudioClip sfx)
    {
        if (audioSource != null && sfx != null)
        {
            audioSource.clip = sfx;
            audioSource.Play();  
        }
    }

    public IEnumerator PlaySFXDelay(AudioSource audioSource, AudioClip sfx, float delay)
    {

        yield return new WaitForSeconds(delay);

        PlaySFX(audioSource, sfx);
    }

}