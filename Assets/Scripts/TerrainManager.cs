using UnityEngine;
using System.Collections.Generic;

public class TerrainManager : MonoBehaviour
{
    public static TerrainManager Instance;

    public GameObject terrainPrefab;
    public int poolSize;
    public float terrainLength;
    public Vector3 terrainStartPos = new Vector3(0f, 0f, -20f); 

    private Queue<GameObject> terrainPool = new Queue<GameObject>();
    private Vector3 nextSpawnPosition;

    void Awake()
    {
        Instance = this;
        nextSpawnPosition = terrainStartPos;
    }

    void Start()
    {
        // Initialize terrain pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject terrain = Instantiate(terrainPrefab, Vector3.zero, Quaternion.identity);
            terrain.SetActive(false);
            terrainPool.Enqueue(terrain);
        }

        // Spawn initial terrains
        for (int i = 0; i < poolSize; i++)
        {
            SpawnNextTerrain();
        }
    }

    public void SpawnNextTerrain()
    {
        // Get a terrain from the pool
        GameObject terrain = terrainPool.Dequeue();
        terrain.SetActive(true);
        terrain.transform.position = nextSpawnPosition;

        // Update the next spawn position
        nextSpawnPosition += Vector3.forward * 10f;

        // Return the terrain to the pool after it moves forward
        terrainPool.Enqueue(terrain);
    }
}
