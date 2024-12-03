using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth, maxThirst, maxHunger;
    public float thirstIncreaseRate, hungerIncreaseRate;
    private float health, thirst, hunger;
    private bool dead;

    [Header("Combat")]
    public float damage;
    public float invincibilityDuration = 1f; // Time before the player can be damaged again
    private float lastDamageTime = -1f;
    public Transform shootPoint; // The position from where bullets are spawned
    public GameObject bulletPrefab; // Bullet prefab
    public float bulletSpeed = 20f; // Speed of the bullet
    public float fireRate = 0.5f; // Time between each shot
    private float nextFireTime = 0f;

    [Header("Interaction")]
    public static bool triggeringWithAI;
    public static GameObject triggeringAI;

    private void Start()
    {
        health = maxHealth;
    }

    private void Update()
    {
        if (dead) return;

        HandleNeeds();
        HandleInteraction();
        HandleShooting();

        if (health <= 0) Die();
    }

    private void HandleNeeds()
    {
        thirst += thirstIncreaseRate * Time.deltaTime;
        hunger += hungerIncreaseRate * Time.deltaTime;

        if (thirst >= maxThirst || hunger >= maxHunger) Die();
    }

    private void HandleInteraction()
    {
        if (triggeringWithAI && triggeringAI)
        {
            if (Input.GetMouseButtonDown(0)) // Left-click to attack
            {
                Attack(triggeringAI);
            }
        }

        if (!triggeringAI)
        {
            triggeringWithAI = false;
        }
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButtonDown(1)) // Right-click to shoot
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Attack(GameObject target)
    {
        if (target.CompareTag("Animal") && target.CompareTag("Enemy"))
        {
            Animal animal = target.GetComponent<Animal>();
            animal.health -= damage;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || shootPoint == null)
        {
            Debug.LogError("Bullet Prefab or Shoot Point is not assigned!");
            return;
        }

        // Instantiate bullet
        Quaternion.identity.Set(0,0,0.5f,0);
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);


        // Get the Rigidbody component from the bullet
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Get the forward direction of the camera
            Vector3 cameraForward = Camera.main.transform.forward;

            // Project the forward direction onto the horizontal plane (y=0)
            Vector3 horizontalDirection = new Vector3(cameraForward.x, cameraForward.y, cameraForward.z).normalized;

            // Apply velocity to the bullet
            rb.linearVelocity = horizontalDirection * bulletSpeed;
        }
    }


    public void Die()
    {
        dead = true;
        print("Player has died");
    }

    public void Drink(float decreaseRate)
    {
        thirst -= decreaseRate;
        thirst = Mathf.Clamp(thirst, 0, maxThirst);
    }
    public void Eat(float decreaseRate)
    {
        thirst -= decreaseRate;
        thirst = Mathf.Clamp(hunger, 0, maxHunger);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Animal"))
        {
            triggeringAI = other.gameObject;
            triggeringWithAI = true;
        }
    }
    public void TakeDamage(float amount)
    {
        if (Time.time - lastDamageTime < invincibilityDuration) return; // Invincibility period
        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        lastDamageTime = Time.time;

        if (health <= 0) Die();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Animal"))
        {
            triggeringAI = null;
            triggeringWithAI = false;
        }
    }
}
