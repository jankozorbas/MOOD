using UnityEngine;

[System.Serializable]
public class Sound
{
    public enum SoundType { Music, SFX }

    public SoundType soundType;
    public AudioClip clip;
    public string name;
    public bool loop;

    [Range(0f, 1f)]
    public float volume;

    [HideInInspector]
    public AudioSource source;
}