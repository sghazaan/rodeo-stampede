using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    public static AnimalSpawner Instance; // Singleton instance
    public Transform player; // Reference to the player
    public float spawnDistance; // Distance ahead of the player to spawn animals
    //public float spawnInterval; // Time between spawns
    public float randomXRange = 4f; // Range for random X spawn positions
    public int initialSpawnCount = 10; // Initial number of animals to spawn
    private float spawnTimer;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Spawn initial set of animals
        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnAnimal();
        }
    }

    // private void Update()
    // {
    //     spawnTimer += Time.deltaTime;

    //     if (spawnTimer >= spawnInterval)
    //     {
    //         SpawnAnimal();
    //         spawnTimer = 0f;
    //     }
    // }

    public void SpawnAnimal()
    {
        // Randomly choose an animal type
        string[] animalTags = { "Bull", "Horse", "Elephant" };
        string selectedTag = animalTags[Random.Range(0, animalTags.Length)];

        // Spawn the animal ahead of the player
        Vector3 spawnPosition = player.position + Vector3.forward * spawnDistance + Vector3.right * Random.Range(-randomXRange, randomXRange);
        GameObject animalObject = ObjectPool.Instance.SpawnFromPool(selectedTag, spawnPosition, Quaternion.identity);

        if (animalObject.TryGetComponent(out Animal animal))
        {
            animal.Initialize(player, animal.speed);
        }
    }
}
