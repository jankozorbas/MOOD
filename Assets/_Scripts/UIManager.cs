using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance {  get; private set; }

    [SerializeField] private TMP_Text keyCountText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text ammoText;
    
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

    private void OnEnable()
    {
        GameManager.OnKeyCountChanged += UpdateKeyUI;
        HealthPack.OnHealthPackPickedUp += UpdateHealthUI;
        PlayerBehavior.OnDamageTaken += UpdateHealthUI;
        Gun.OnAmmoChanged += UpdateAmmoUI;
        GunSwitcher.OnWeaponChanged += UpdateAmmoUI;
    }

    private void OnDisable()
    {
        GameManager.OnKeyCountChanged -= UpdateKeyUI;
        HealthPack.OnHealthPackPickedUp -= UpdateHealthUI;
        PlayerBehavior.OnDamageTaken -= UpdateHealthUI;
        Gun.OnAmmoChanged -= UpdateAmmoUI;
        GunSwitcher.OnWeaponChanged -= UpdateAmmoUI;
    }
}