using UnityEngine;

public class TerrainTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TerrainEnd"))
        {
            // Trigger terrain pooling/spawning logic when reaching the end
            Debug.Log("Player Reached the end of the terrain!");
            TerrainManager.Instance.SpawnNextTerrain();
        }
    }
}