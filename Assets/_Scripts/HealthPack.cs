using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    private PlayerBehavior playerBehavior;

    [SerializeField] private int minHealthIncreaseAmount = 25;
    [SerializeField] private int maxHealthIncreaseAmount = 36;

    public static Action<int> OnHealthPackPickedUp;

    private void Awake()
    {
        playerBehavior = FindObjectOfType<PlayerBehavior>().GetComponent<PlayerBehavior>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerBehavior.playerHealth == playerBehavior.maxHealth)
            {
                UIManager.Instance.ShowInteractionMessage("Max health reached");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.HideInteractionMessage();
        }
    }

    public void RegenerateHealth()
    {
        int healthPackIncrease = UnityEngine.Random.Range(minHealthIncreaseAmount, maxHealthIncreaseAmount);
        playerBehavior.playerHealth = Mathf.Clamp(playerBehavior.playerHealth + healthPackIncrease, 0, playerBehavior.maxHealth);
        OnHealthPackPickedUp?.Invoke(playerBehavior.playerHealth);
        AudioManager.Instance.PlaySound("HealthPickup");
    }
}