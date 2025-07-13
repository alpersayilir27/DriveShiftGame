using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public SoundLibrary soundLibrary;
    public AudioSource musicSource;
    public AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            soundLibrary.Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(string soundName)
    {
        AudioClip clip = soundLibrary.GetClip(soundName);
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayMusic(string soundName, bool loop = true)
    {
        AudioClip clip = soundLibrary.GetClip(soundName);
        if (clip != null)
        {
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}
