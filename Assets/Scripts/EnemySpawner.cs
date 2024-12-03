using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Spawning Settings")]
    [SerializeField] private GameObject enemyPrefab; // Enemy prefab to spawn
    [SerializeField] private Transform[] spawnPoints; // Array of spawn points
    [SerializeField] private float spawnInterval = 5f; // Time interval between spawns
    [SerializeField] private int maxEnemies = 10; // Maximum number of enemies allowed

    private int currentEnemyCount;

    private void Start()
    {
        // Validate prefab assignment
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab is not assigned in EnemySpawner!");
            enabled = false;
            return;
        }

        // Validate spawn points
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned in EnemySpawner!");
            enabled = false;
            return;
        }

        // Start the spawn loop
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Check if maximum enemy count is reached
            if (currentEnemyCount >= maxEnemies) continue;

            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        // Pick a random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Instantiate enemy at the chosen spawn point
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // Increment enemy count
        currentEnemyCount++;

        // Register enemy's destruction to decrease count
        enemy.GetComponent<Enemy>().OnEnemyDestroyed += HandleEnemyDestroyed;
    }

    private void HandleEnemyDestroyed()
    {
        currentEnemyCount--;
    }

    // Optionally, for debugging, draw spawn points in the editor
    private void OnDrawGizmos()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Gizmos.color = Color.red;
            foreach (var point in spawnPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawSphere(point.position, 0.5f);
                }
            }
        }
    }
}
