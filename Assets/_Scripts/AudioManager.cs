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
        PlaySound("Ambience");
    }

    private void InitializeSounds()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;

            s.source.spatialBlend = s.is3D ? 1f : 0f;
            s.source.minDistance = s.minDistance;
            s.source.maxDistance = s.maxDistance;
            s.source.rolloffMode = AudioRolloffMode.Linear;
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

    public void PlaySoundAtPosition(string name, Vector3 position)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound '" + name + "' not found.");
            return;
        }

        if (!s.is3D)
        {
            Debug.LogWarning("Sound '" + name + "' is not a 3D sound.");
            return;
        }

        GameObject tempAudioObject = new GameObject($"TempAudio_{name}");
        tempAudioObject.transform.position = position;
        AudioSource tempSource = tempAudioObject.AddComponent<AudioSource>();

        tempSource.clip = s.clip;
        tempSource.volume = s.volume;
        tempSource.spatialBlend = 1f;
        tempSource.minDistance = s.minDistance;
        tempSource.maxDistance = s.maxDistance;
        tempSource.rolloffMode = AudioRolloffMode.Linear;

        tempSource.Play();
        Destroy(tempAudioObject, s.clip.length);
    }

    public void PlayFootstepSounds()
    {
        string[] footstepSounds = { "Footstep_01", "Footstep_02", "Footstep_03", "Footstep_04", "Footstep_05" };
        string randomFootstep = footstepSounds[UnityEngine.Random.Range(0, footstepSounds.Length)];
        PlaySound(randomFootstep);
    }

    public void PlayEnemyFootstepSounds(Vector3 position)
    {
        string[] footstepSounds = { "EnemyFootstep_01", "EnemyFootstep_02", "EnemyFootstep_03" };
        string randomFootstep = footstepSounds[UnityEngine.Random.Range(0, footstepSounds.Length)];
        PlaySoundAtPosition(randomFootstep, position);
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