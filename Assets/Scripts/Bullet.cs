using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f; // How long the bullet exists before being destroyed
    public float damage = 10f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Animal"))
        {
            Animal animal = collision.gameObject.GetComponent<Animal>();
            if (animal != null)
            {
                animal.health -= damage;
            }
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.health -= damage;
            }
        }

        // Destroy the bullet on collision
        Destroy(gameObject);
    }
}
