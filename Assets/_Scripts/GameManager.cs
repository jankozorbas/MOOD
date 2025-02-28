using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static Action<int> OnKeyCountChanged;

    [SerializeField] private float timePerRound = 300f;

    private int keyCount = 0;
    private float timeLeft = 0f;
    private bool isTimerActive;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitializeTimer();
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
                isTimerActive = false;
                Debug.Log("You died.");
                //subscribe to death event
            }
        }
    }

    public void WinGame()
    {
        isTimerActive = false;
        Debug.Log("You Win.");

        //something happens
    }
}