using UnityEngine;

[System.Serializable]
public class Sound
{
    //public enum SoundType { Music, SFX }

    public AudioClip clip;
    //public SoundType soundType;
    public string name;
    public bool loop;

    [Range(0f, 1f)]
    public float volume;

    [HideInInspector]
    public AudioSource source;
}