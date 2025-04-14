using System.Collections;
using TMPro;
using UnityEngine;

public class SubtitleManager : MonoBehaviour
{
    public static SubtitleManager Instance;

    [SerializeField] private TMP_Text subtitleText;
    [SerializeField] private float displayTime = 3f;

    private Coroutine subtitleCoroutine;

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
    }

    public void ShowSubtitle(string subText, float duration)
    {
        if (subtitleCoroutine != null) StopCoroutine(subtitleCoroutine);

        subtitleText.text = subText;
        subtitleText.gameObject.SetActive(true);
        subtitleCoroutine = StartCoroutine(HideAfter(duration > 0f ? duration : displayTime));
    }

    private IEnumerator HideAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        subtitleText.gameObject.SetActive(false);
    }
}