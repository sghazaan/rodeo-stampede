using UnityEngine;
using System.Collections.Generic;

public class AnimalSpawner : MonoBehaviour
{
    public GameObject animalPrefab;
    public int poolSize = 10;
    public float spawnInterval = 2f;
    public Transform spawnPoint;

    private List<GameObject> animalPool;
    private float timer;

    void Start()
    {
        animalPool = new List<GameObject>();

        // Initialize the pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject animal = Instantiate(animalPrefab);
            animal.SetActive(false);
            animalPool.Add(animal);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnAnimal();
            timer = 0;
        }
    }

    void SpawnAnimal()
    {
        foreach (GameObject animal in animalPool)
        {
            if (!animal.activeInHierarchy)
            {
                animal.transform.position = spawnPoint.position;
                animal.transform.rotation = spawnPoint.rotation;
                animal.SetActive(true);
                return;
            }
        }
    }
}
