using UnityEngine;
using System.Collections.Generic;

public class SpawnScript : MonoBehaviour
{
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private int maxZombies= 10;
    [SerializeField] private float minSpawnInterval = 3f;
    [SerializeField] private float maxSpawnInterval = 7f;
    [SerializeField] private Transform player;
    [SerializeField] private float minDistanceFromPlayer = 5f;
    [SerializeField] private LayerMask obstacleLayer;

    [SerializeField] private Vector3 spawnAreaMin = new Vector3(-10, 0, -10);
    [SerializeField] private Vector3 spawnAreaMax = new Vector3(10, 0, 10);

    private List<GameObject> spawnedZombies = new List<GameObject>();
    private float spawnTimer;
    private float currentSpawnInterval;

    private void Start()
    {
        currentSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    private void Update()
    {
        spawnedZombies.RemoveAll(zombie => zombie == null);

        int currentZombieCount = spawnedZombies.Count;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= currentSpawnInterval && currentZombieCount < maxZombies)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();

            if (spawnPosition != Vector3.zero)
            {
                GameObject newZombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
                spawnedZombies.Add(newZombie);
            }

            spawnTimer = 0f;
            currentSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y),
                Random.Range(spawnAreaMin.z, spawnAreaMax.z)
            );

            if (Vector3.Distance(randomPosition, player.position) > minDistanceFromPlayer)
            {
                if (!Physics.CheckBox(randomPosition, Vector3.one * 0.5f, Quaternion.identity, obstacleLayer))
                {
                    return randomPosition;
                }
            }
        }

        return Vector3.zero;
    }
}