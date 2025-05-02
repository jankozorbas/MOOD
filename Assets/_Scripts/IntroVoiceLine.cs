using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroVoiceLine : MonoBehaviour
{
    [SerializeField] private string voiceLineName;
    [TextArea(5, 10)]
    [SerializeField] private string subtitleText;
    [SerializeField] private float subtitleDuration = 3f;
    [SerializeField] private bool triggerOnce = true;

    private SubtitleManager subtitleManager;
    private static bool hasTriggered = false;

    private void Awake()
    {
        subtitleManager = FindObjectOfType<SubtitleManager>();
    }

    private void Start()
    {
        if (triggerOnce && hasTriggered) return;

        hasTriggered = true;

        AudioManager.Instance.PlaySound(voiceLineName);
        subtitleManager.ShowSubtitle(subtitleText, subtitleDuration);
    }
}