using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 10f;
    public float forwardForce = 5f;
    public Animator animator; // Assign an animator for idle and jump animations
    private Rigidbody rb;
    private bool isRiding = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Riding mechanic
        if (Input.GetMouseButton(0)) // Hold mouse button to ride
        {
            if (!isRiding)
            {
                isRiding = true;
                animator.SetBool("isRiding", true);
            }
        }
        else if (isRiding) // Release to jump off
        {
            isRiding = false;
            animator.SetBool("isRiding", false);

            // Jump forward
            rb.velocity = transform.forward * forwardForce + Vector3.up * jumpForce;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Animal"))
        {
            // Parent player to the animal
            transform.parent = collision.transform;
            rb.isKinematic = true;
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            // Reset on ground
            transform.parent = null;
            rb.isKinematic = false;
        }
    }
}
