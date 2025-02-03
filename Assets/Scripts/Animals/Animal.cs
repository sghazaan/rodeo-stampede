using UnityEngine;

public class Animal : MonoBehaviour
{
    public float speed;
    public float ridingYOffset;
    public bool isRidden = false;
    protected Transform player;
    public float respawnThreshold = 30f;

    public virtual void Initialize(Transform playerTransform, float animalSpeed)
    {
        player = playerTransform;
        speed = animalSpeed;
        isRidden = false;
        //set animals's rotation to identity
        transform.rotation = Quaternion.identity;
    }

    protected virtual void Update()
    {
        if(GameManager.IsGameOver)
        {
            return;
        }

        if (isRidden)
        {
            // Follow the player position
            transform.position = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, Time.deltaTime * 5f);
        }
        else
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.World);
        }

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
                GameHandler.Instance.GameOver();
                return;
            }
            isRidden = true;
            EventHub.InvokeAnimalRidden(ridingYOffset, this);
        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("Animal"))
        {
            if(GameManager.IsPlayerRiding && other.gameObject.GetComponent<Animal>().isRidden)
            {
                GameHandler.Instance.GameOver();
            }
        }
        else if(other.CompareTag("Hurdle"))
        {
            if(isRidden)
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
