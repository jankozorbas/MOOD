using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int pistolDamage = 2;
    [SerializeField] private int rifleDamage = 5;

    private PlayerBehavior playerBehavior;

    private void Awake()
    {
        playerBehavior = FindObjectOfType<PlayerBehavior>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerBehavior.TakeDamage(pistolDamage);
            Destroy(gameObject);
        }
        else if (collision != null)
            Destroy(gameObject);
        else 
            Destroy(gameObject, 3f);
    }
}