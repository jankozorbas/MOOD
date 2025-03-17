using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance {  get; private set; }

    [SerializeField] private TMP_Text keyCountText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private Image damageIndicator;
    
    public TMP_Text interactionText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateKeyUI(GameManager.Instance.GetKeyCount()); 
        UpdateHealthUI(FindObjectOfType<PlayerBehavior>().GetComponent<PlayerBehavior>().playerHealth);

        interactionText.gameObject.SetActive(false);
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