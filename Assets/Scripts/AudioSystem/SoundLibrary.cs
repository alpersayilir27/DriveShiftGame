using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Audio/SoundLibrary")]
public class SoundLibrary : ScriptableObject
{
    public List<Sound> sounds;

    private Dictionary<string, AudioClip> soundDict;

    public void Init()
    {
        soundDict = new Dictionary<string, AudioClip>();
        foreach (var sound in sounds)
        {
            if (!soundDict.ContainsKey(sound.name))
            {
                soundDict.Add(sound.name, sound.clip);
            }
        }
    }

    public AudioClip GetClip(string name)
    {
        if (soundDict == null) Init();

        if (soundDict.TryGetValue(name, out AudioClip clip))
        {
            return clip;
        }

        Debug.LogWarning($"Sound '{name}' not found in library!");
        return null;
    }
}
