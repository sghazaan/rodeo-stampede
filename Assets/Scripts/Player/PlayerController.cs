using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody playerRigidbody; // Assign the player's Rigidbody in the Inspector
    public float jumpForce = 5f;      // Jump force to apply
    [SerializeField] private float moveSpeed;
    [SerializeField] private float dragSensitivity;
    private bool isHolding = false; // To track if the user is holding down
    private bool isGrounded;
    private Vector2 touchStartPos; // To store the position where touch started
    private Vector2 touchCurrentPos; // To store the current touch position

    void Update()
    {
        // Move the player forward
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
         // Primary Down: Touch starts
        if (Input.GetMouseButtonDown(0))
        {
            isHolding = true;
            touchStartPos = Input.mousePosition; // Save the starting position
            Debug.Log("Touch Started at: " + touchStartPos);
        }
        // Primary Down Stay: Detect dragging
        if (Input.GetMouseButton(0) && isHolding)
        { 
            touchCurrentPos = Input.mousePosition; // Update the current position

            float dragDistance = touchCurrentPos.x - touchStartPos.x;
            if (Mathf.Abs(dragDistance) > 50) // Adjust threshold for dragging
            {
                if (dragDistance > 0)
                {
                    transform.Translate(Vector3.right * moveSpeed * Time.deltaTime * dragSensitivity);
                    Debug.Log("Dragging Right");
                }
                else
                {
                    transform.Translate(-Vector3.right * moveSpeed * Time.deltaTime * dragSensitivity);
                    Debug.Log("Dragging Left");
                }

                // Reset the start position to prevent continuous reporting
                touchStartPos = touchCurrentPos;
            }
        }

        // Primary Up: Touch ends
        if (Input.GetMouseButtonUp(0))
        {
            isHolding = false;
            Debug.Log("Touch Ended at: " + Input.mousePosition);
            if (isGrounded)
            {
               playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0f, playerRigidbody.velocity.z); // Reset vertical velocity
                playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply jump force
            }
        }
    }

    

    // Detect ground collision
    // private void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("Terrain")) 
    //     {
    //         isGrounded = true;
    //     }
    // }

    // private void OnCollisionExit(Collision collision)
    // {
    //     if (collision.gameObject.CompareTag("Terrain")) 
    //     {
    //         isGrounded = false;
    //     }
    // }
}
