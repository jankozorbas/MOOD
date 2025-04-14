using UnityEngine;

public class Key : MonoBehaviour
{
    private AudioSource emitter;

    private void Start()
    {
        emitter = AudioManager.Instance.PlaySoundAtPosition("KeyNoise", transform.position);

        if (emitter != null)
        {
            emitter.transform.parent = transform;
        }
    }

    public void StopSoundEmitter()
    {
        if (emitter != null)
        {
            AudioManager.Instance.FadeOutAndDestroy(emitter, .5f);
        }
    }
}