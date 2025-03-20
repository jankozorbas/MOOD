using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance {  get; private set; }

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private TMP_Text keyCountText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private Image damageIndicator;
    [SerializeField] private Slider sensitivitySlider;

    private MouseMovement mouseMovement;
    private bool isPaused = false;

    public TMP_Text interactionText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        mouseMovement = FindObjectOfType<MouseMovement>();
    }

    private void Start()
    {
        UpdateKeyUI(GameManager.Instance.GetKeyCount()); 
        UpdateHealthUI(FindObjectOfType<PlayerBehavior>().GetComponent<PlayerBehavior>().playerHealth);

        interactionText.gameObject.SetActive(false);

        LoadSensitivity();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused) PauseGame();
            else ResumeGame();
        }
    }

    private void PauseGame()
    {
        pauseMenu.SetActive(true);
        isPaused = true;
        AudioListener.pause = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        pauseMenu.SetActive(false);
        isPaused = false;
        AudioListener.pause = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void UpdateSliderValue(float newValue)
    {
        float roundedValue = Mathf.Round(newValue * 10f) / 10f;

        mouseMovement.MouseSensitivity = roundedValue;
        sensitivitySlider.value = roundedValue;
        PlayerPrefs.SetFloat("MouseSensitivity", roundedValue);
        PlayerPrefs.Save();
    }

    private void LoadSensitivity()
    {
        // Load saved sensitivity or use default 1f
        float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
        // Apply it to the slider
        sensitivitySlider.value = savedSensitivity;
        // Apply it to the actual sensitivity variable;
        mouseMovement.MouseSensitivity = savedSensitivity;
        // Ensure changer are applied when the player interacts with the slider
        sensitivitySlider.onValueChanged.AddListener(UpdateSliderValue);
    }

    private void UpdateKeyUI(int keyCount)
    {
        keyCountText.text = "keys: " + keyCount.ToString();
    }

    private void UpdateHealthUI(int health)
    {
        healthText.text = "health: " + health.ToString();
    }

    private void UpdateAmmoUI(int currentAmmo, int reserveAmmo)
    {
        ammoText.text = $"{currentAmmo} / {reserveAmmo}";
    }

    public void UpdateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60f);
        float seconds = Mathf.FloorToInt(currentTime % 60f);

        timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }

    public void ShowDamageIndicator(int timeDisplayed)
    {
        StartCoroutine(FlashRed());
    }

    private IEnumerator FlashRed()
    {
        float maxAlpha = .5f;
        float fadeDuration = .5f;
        
        Color color = damageIndicator.color;
        color.a = maxAlpha;
        damageIndicator.color = color;

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(maxAlpha, 0, elapsedTime / fadeDuration);
            damageIndicator.color = color;
            yield return null;
        }
    }

    private void OnEnable()
    {
        GameManager.OnKeyCountChanged += UpdateKeyUI;
        HealthPack.OnHealthPackPickedUp += UpdateHealthUI;
        PlayerBehavior.OnDamageTaken += UpdateHealthUI;
        PlayerBehavior.OnDamageTaken += ShowDamageIndicator;
        Gun.OnAmmoChanged += UpdateAmmoUI;
        GunSwitcher.OnWeaponChanged += UpdateAmmoUI;
    }

    private void OnDisable()
    {
        GameManager.OnKeyCountChanged -= UpdateKeyUI;
        HealthPack.OnHealthPackPickedUp -= UpdateHealthUI;
        PlayerBehavior.OnDamageTaken -= UpdateHealthUI;
        PlayerBehavior.OnDamageTaken -= ShowDamageIndicator;
        Gun.OnAmmoChanged -= UpdateAmmoUI;
        GunSwitcher.OnWeaponChanged -= UpdateAmmoUI;
    }
}