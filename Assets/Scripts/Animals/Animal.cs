using UnityEngine;

public class Animal : MonoBehaviour
{
    public float speed; // Speed of the animal
    public bool isRidden = false; // Whether the player is riding this animal
    protected Transform player; // Reference to the player transform
    public float respawnThreshold = 30f; // Distance behind the player to deactivate
    bool isRidingDone = false; // Whether the player has finished riding

    public virtual void Initialize(Transform playerTransform, float animalSpeed)
    {
        player = playerTransform;
        speed = animalSpeed;
        isRidden = false;
    }

    protected virtual void Update()
    {
        if(GameManager.IsGameLost)
        {
            return;
        }
        if (isRidden)
        {
            // Follow the player position
            transform.position = new Vector3(player.position.x, transform.position.y, player.position.z);
        }
        else
        {
            // Move forward and slightly sway left or right
            Vector3 forward = transform.forward;
            forward.x += Random.Range(-0.02f, 0.02f); // Slight sway
            transform.Translate(forward.normalized * speed * Time.deltaTime, Space.World);
        }

        // Check if the animal falls behind the player
        if (player.position.z - transform.position.z > respawnThreshold)
        {
            DeactivateAndRespawn();
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(!isRidden && GameManager.IsPlayerRiding)
            {
                GameManager.IsGameLost = true;
                Debug.Log("Game Over! Animal - Player");
                return;
            }
            isRidden = true;
            EventHub.InvokeAnimalRidden(transform.position.y, this);
        }
        //compare layer to amimal layer
        else if(other.gameObject.layer == LayerMask.NameToLayer("Animal"))
        {
            if(GameManager.IsPlayerRiding)
            {
                if(other.gameObject.GetComponent<Animal>().isRidden)
                {
                    GameManager.IsGameLost = true;
                    Debug.Log("Game Over! Animal - Animal");
                }
            }
        }
    }

    private void DeactivateAndRespawn()
    {
        // Deactivate this animal
        gameObject.SetActive(false);

        // Respawn a new animal through the ObjectPool
        AnimalSpawner.Instance.SpawnAnimal();
    }
}
