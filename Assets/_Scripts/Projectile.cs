using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int minDamage = 15;
    [SerializeField] private int maxDamage = 21;

    private PlayerBehavior playerBehavior;

    private void Awake()
    {
        playerBehavior = FindObjectOfType<PlayerBehavior>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (playerBehavior.playerHealth > 0)
            {
                playerBehavior.TakeDamage(Random.Range(minDamage, maxDamage));
                Destroy(gameObject);
            }
        }
        else if (collision != null)
            Destroy(gameObject);
        else 
            Destroy(gameObject, 3f);
    }
}