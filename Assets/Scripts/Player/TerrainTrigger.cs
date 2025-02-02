using UnityEngine;

public class TerrainTrigger : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TerrainEnd"))
        {
            // Trigger terrain pooling/spawning logic when reaching the end
            //Debug.Log("Player Reached the end of the terrain!");
            TerrainManager.Instance.SpawnNextTerrain();
        }
        // else if(other.gameObject.layer == LayerMask.NameToLayer("Animal"))
        // {
        //     Debug.Log("Player collided with animal");
        //    playerController.PlayerLandedOnAnimal();
        // }
    }
}