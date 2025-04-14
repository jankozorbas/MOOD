using UnityEngine;

public class VoicelineTrigger : MonoBehaviour
{
    [SerializeField] private string voiceLineName;
    [SerializeField] private string subtitleText;
    [SerializeField] private float subtitleDuration;
    [SerializeField] private bool triggerOnce = true;

    private SubtitleManager subtitleManager;
    private bool hasTriggered = false;

    private void Awake()
    {
        subtitleManager = FindObjectOfType<SubtitleManager>();
    }

    private void Start()
    {
        hasTriggered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (triggerOnce && hasTriggered) return;

        hasTriggered = true;

        AudioManager.Instance.PlaySound(voiceLineName);
        subtitleManager.ShowSubtitle(subtitleText, subtitleDuration);
    }
}