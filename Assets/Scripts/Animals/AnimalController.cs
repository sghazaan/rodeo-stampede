using UnityEngine;

public class AnimalController : MonoBehaviour
{
    public float speed = 5f;
    private float boundary = -10f; // Boundary for respawning animals

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Check if out of bounds
        if (transform.position.z < boundary)
        {
            gameObject.SetActive(false); // Deactivate for object pooling
        }
    }
}
