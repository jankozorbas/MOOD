using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static Action<int> OnKeyCountChanged;

    [SerializeField] private float timePerRound = 300f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private int keysNeeded = 4;
    [SerializeField] private Image fadeImage;
    [SerializeField] private GameObject winTextObject;
    [SerializeField] private GameObject loseTextObject;

    private int keyCount = 0;
    private float timeLeft = 0f;
    private bool isTimerActive;

    public int KeysNeeded => keysNeeded;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitializeTimer();
        StartCoroutine(FadeIn());
    }

    private void Update()
    {
        Timer();
    }

    public void AddKey()
    {
        keyCount++;
        OnKeyCountChanged?.Invoke(keyCount);
    }

    public void RemoveKey()
    {
        keyCount--;
        OnKeyCountChanged?.Invoke(keyCount);
    }

    public int GetKeyCount() => keyCount;

    private void InitializeTimer()
    {
        isTimerActive = true;
        timeLeft = timePerRound;
    }

    private void Timer()
    {
        if (isTimerActive)
        {
            if (timeLeft > 0f)
            {
                timeLeft -= Time.deltaTime;
                UIManager.Instance.UpdateTimer(timeLeft);
            }
            else
            {
                timeLeft = 0f;
                FindObjectOfType<PlayerBehavior>().GetComponent<PlayerBehavior>().CanMove = false; // do it in a better way
                LoseGame(); 
                //subscribe to death event
            }
        }
    }

    public void WinGame()
    {
        // Stop timer
        isTimerActive = false;
        // Fade to black
        StartCoroutine(FadeOut());
        // UI says you win
        StartCoroutine(WinLoseText(winTextObject, 1.2f));
        // Reset the game after 5 seconds
        Invoke("ReloadScene", 5f);
        // Later Voice Over will be added and sound effects of stopping the bomb etc.
        Debug.Log("You Win.");
    }

    public void LoseGame()
    {
        // Stop timer
        isTimerActive = false;
        // Fade to black
        StartCoroutine(FadeOut());
        // UI says you lose
        StartCoroutine(WinLoseText(loseTextObject, 1.2f));
        // Reset the game after 5 seconds
        Invoke("ReloadScene", 5f);
        // Later Voice Over will be added and sound effects of stopping the bomb animation of the bomb leaving the silo etc.
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ShowWinLoseText(GameObject textToDisplay)
    {
        textToDisplay.SetActive(true);
    }

    private IEnumerator WinLoseText(GameObject winLoseText, float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowWinLoseText(winLoseText);
    }

    private IEnumerator FadeIn()
    {
        yield return Fade(1, 0);
    }
    
    private IEnumerator FadeOut()
    {
        yield return Fade(0, 1);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
    }
}