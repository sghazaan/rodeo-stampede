using UnityEngine;

public class TerrainSpawner : MonoBehaviour
{
    public GameObject terrainPrefab;
    public int poolSize = 5;
    public float terrainLength = 30f;

    private GameObject[] terrainPool;
    private int currentIndex = 0;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        terrainPool = new GameObject[poolSize];

        // Initialize the terrain pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject terrain = Instantiate(terrainPrefab);
            terrain.transform.position = new Vector3(0, 0, i * terrainLength);
            terrainPool[i] = terrain;
        }
    }

    void Update()
    {
        // Check if player has passed the terrain piece
        if (player.position.z > terrainPool[currentIndex].transform.position.z + terrainLength)
        {
            // Move terrain to the end of the queue
            GameObject terrain = terrainPool[currentIndex];
            terrain.transform.position += Vector3.forward * terrainLength * poolSize;

            // Update index
            currentIndex = (currentIndex + 1) % poolSize;
        }
    }
}
