using UnityEngine;

public class Animal : MonoBehaviour
{
    public float speed;
    public bool isRidden = false;
    protected Transform player;
    public float respawnThreshold = 30f;
    private Vector3 lastPosition;
    private float smoothTime = 0.1f;
    private Vector3 currentVelocity;

    public virtual void Initialize(Transform playerTransform, float animalSpeed)
    {
        player = playerTransform;
        speed = animalSpeed;
        isRidden = false;
        lastPosition = transform.position;
    }

    protected virtual void Update()
    {
        if(GameManager.IsGameOver)
        {
            return;
        }

        if (isRidden)
        {
            // Smoothly follow player's XZ position while maintaining Y
            Vector3 targetPosition = new Vector3(
                player.position.x, 
                transform.position.y, 
                player.position.z
            );
            
            // Use SmoothDamp for smoother following
            transform.position = Vector3.SmoothDamp(
                transform.position, 
                targetPosition, 
                ref currentVelocity, 
                smoothTime
            );
        }
        else
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.World);
        }

        if (player.position.z - transform.position.z > respawnThreshold)
        {
            DeactivateAndRespawn();
        }
        
        lastPosition = transform.position;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(!isRidden && GameManager.IsPlayerRiding)
            {
                GameHandler.Instance.GameOver();
                return;
            }
            isRidden = true;
            EventHub.InvokeAnimalRidden(transform.position.y, this);
        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("Animal"))
        {
            if(GameManager.IsPlayerRiding && other.gameObject.GetComponent<Animal>().isRidden)
            {
                GameHandler.Instance.GameOver();
            }
        }
    }

    private void DeactivateAndRespawn()
    {
        gameObject.SetActive(false);
        AnimalSpawner.Instance.SpawnAnimal();
    }
}
