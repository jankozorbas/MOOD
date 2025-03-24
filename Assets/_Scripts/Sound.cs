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

    [Header("3D settings")]
    public bool is3D = false;
    public float spatialBlend = 1f;
    public float minDistance = .2f;
    public float maxDistance = 500f;

    [HideInInspector]
    public AudioSource source;
}