using UnityEngine;
using System;
using static Sound;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private Sound[] sounds;

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
            return;
        }

        InitializeSounds();
    }

    private void Start()
    {
        PlaySound("Placeholder");
    }

    private void InitializeSounds()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }
    }

    public void PlaySound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound '" + name + "' not found.");
            return;
        }

        // Change when UI Sounds are added
        if (UIManager.Instance.IsPaused) return;

        switch (s.soundType)
        {
            case SoundType.SFX:
                s.source.PlayOneShot(s.clip);
                break;
            case SoundType.Music:
                if (!s.source.isPlaying) s.source.Play();
                break;
            default:
                return;
        }
    }

    public void StopSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound '" + name + "' not found.");
            return;
        }

        if (s.soundType == SoundType.Music || s.source.loop)
        {
            s.source.Stop();
            s.source.time = 0f;
        }
        else
        {
            Debug.LogWarning("Cannot stop SFX '" + name + "' because it was played by using PlayOneShot().");
        }
    }

    public bool IsPlaying(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null) return false;

        if (s.soundType == SoundType.SFX)
        {
            Debug.LogWarning("Cannot check if it's playing because it was played by using PlayOneShot().");
            return false;
        }

        return s.source.isPlaying;
    }
}