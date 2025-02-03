using UnityEngine;
using System.Collections.Generic;

public class AnimalSpawner : MonoBehaviour
{
    public static AnimalSpawner Instance;

    public Transform player;
    public float spawnDistance = 50f; // Base spawn distance
    public float minSpawnInterval = 10f; // Minimum z-axis spacing between animals
    public float maxSpawnInterval = 20f; // Maximum z-axis spacing between animals

    // Fixed X-axis positions for spawning
    private float[] xPositions = { -4f, 4f };
    
    // Track spawned animal positions to avoid overlap
    private List<Vector3> spawnedPositions = new List<Vector3>();

    public int initialSpawnCount = 10;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnInitialAnimals()
    {
        // Clear any previous spawn tracking
        spawnedPositions.Clear();
        
        // Spawn initial set of animals
        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnAnimal();
        }
    }

    // public void SpawnAnimal()
    // {
    //     // Randomly choose an animal type
    //     string[] animalTags = { "Bull", "Horse", "Elephant" };
    //     string selectedTag = animalTags[Random.Range(0, animalTags.Length)];

    //     // Find a suitable spawn position
    //     Vector3 spawnPosition = GetUniqueSpawnPosition();

    //     GameObject animalObject = ObjectPool.Instance.SpawnFromPool(selectedTag, spawnPosition, Quaternion.identity);

    //     if (animalObject.TryGetComponent(out Animal animal))
    //     {
    //         animal.Initialize(player, animal.speed);
    //     }
    // }


    public void SpawnAnimal()
    {
        // Randomly choose an animal type
        string[] animalTags = { "Bull", "Horse", "Elephant" };
        string selectedTag;
        GameObject animalObject;
        Animal animal;

        int maxAttempts = 10; // Prevent infinite loop
        int attempts = 0;

        do
        {
            selectedTag = animalTags[Random.Range(0, animalTags.Length)];
            animalObject = ObjectPool.Instance.SpawnFromPool(selectedTag, GetUniqueSpawnPosition(), Quaternion.identity);
            
            if (animalObject.TryGetComponent(out animal))
            {
                // If animal is not currently being ridden, initialize and break
                if (!animal.isRidden)
                {
                    animal.Initialize(player, animal.speed);
                    break;
                }
            }

            // If animal is ridden, deactivate and try again
            animalObject.SetActive(false);
            attempts++;
        }
        while (attempts < maxAttempts);

        // Fallback if no non-ridden animal found
        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("Could not find a non-ridden animal to spawn");
        }
    }

    private Vector3 GetUniqueSpawnPosition()
    {
        int attempts = 0;
        Vector3 basePosition = player.position + Vector3.forward * spawnDistance;
        
        while (attempts < 100) // Prevent infinite loop
        {
            // Choose random x position from fixed array
            float xPos = xPositions[Random.Range(0, xPositions.Length)];
            
            // Add random z-axis interval
            float zInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            
            Vector3 potentialPosition = new Vector3(
                xPos, 
                basePosition.y, 
                basePosition.z + zInterval * (attempts + 1)
            );

            // Check if position is unique
            if (IsPositionUnique(potentialPosition))
            {
                spawnedPositions.Add(potentialPosition);
                return potentialPosition;
            }

            attempts++;
        }

        // Fallback to a default position if unique position can't be found
        return basePosition + new Vector3(
            xPositions[Random.Range(0, xPositions.Length)], 
            0, 
            spawnDistance
        );
    }

    private bool IsPositionUnique(Vector3 position, float minDistance = 3f)
    {
        foreach (Vector3 spawnedPos in spawnedPositions)
        {
            // Check both x and z positions for overlap
            if (Mathf.Abs(position.x - spawnedPos.x) < minDistance &&
                Mathf.Abs(position.z - spawnedPos.z) < minDistance)
            {
                return false;
            }
        }
        return true;
    }
}