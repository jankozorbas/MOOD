using UnityEngine;

public class Projectile : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null) Destroy(gameObject);
        
        Destroy(gameObject, 5f);
    }
}