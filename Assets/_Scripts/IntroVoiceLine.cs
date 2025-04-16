using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroVoiceLine : MonoBehaviour
{
    [SerializeField] private string voiceLineName;
    [TextArea(5, 10)]
    [SerializeField] private string subtitleText;
    [SerializeField] private float subtitleDuration = 3f;

    private SubtitleManager subtitleManager;

    private void Awake()
    {
        subtitleManager = FindObjectOfType<SubtitleManager>();
    }

    private void Start()
    {
        AudioManager.Instance.PlaySound(voiceLineName);
        subtitleManager.ShowSubtitle(subtitleText, subtitleDuration);
    }
}