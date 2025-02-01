using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody playerRigidbody; // Assign in Inspector
    public float jumpForce;      // Jump force to apply
    [SerializeField] private float moveSpeed;
    [SerializeField] private float dragSensitivity;
    
    private bool isHolding = false;   // Tracks if the user is holding down
    private bool canJump = false;     // Only allow jumping when user releases after holding
    private bool isJumping = false;   // Tracks if the player is currently in the air

    private Vector2 touchStartPos;    // Store the position where touch started
    private Vector2 touchCurrentPos;  // Store the current touch position

    float riddenYPos; // Y position of the player when riding an animal
    bool isRiding = false; // Whether the player is riding an animal

    void OnEnable()
    {
        EventHub.OnAnimalRidden += OnAnimalRidden;
    }

    void OnDisable()
    {
        EventHub.OnAnimalRidden -= OnAnimalRidden;
    }
    void Update()
    {
        // Move the player forward
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        if(isRiding)
        {
            transform.position = new Vector3(transform.position.x, riddenYPos, transform.position.z);
        }

        // Detect touch start
        if (Input.GetMouseButtonDown(0))
        {
            isHolding = true;
            touchStartPos = Input.mousePosition;
            
            // Only allow jumping if the player is grounded
            if (!isJumping)
            {
                canJump = true;  // Enable jump when releasing
            }

            Debug.Log("Touch Started at: " + touchStartPos);
        }

        Dragging();

        // Detect touch release & trigger jump
        if (isHolding && !Input.GetMouseButton(0))
        {
            isHolding = false;

            // Jump only if we were allowed to jump before release
            if (canJump && !isJumping)
            {
                Jump();
            }

            // Reset jump permission so it doesn't trigger multiple times
            canJump = false;
        }
    }

    private void Jump()
    {
        Debug.Log("Jumping");
        playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0f, playerRigidbody.velocity.z);
        playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isJumping = true;  // Set jumping state
    }

    // Detect ground collision
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            isJumping = false;  // Allow jumping again when landed
        }
    }

    void Dragging()
    {
        // Detect dragging
        if (Input.GetMouseButton(0) && isHolding)
        { 
            touchCurrentPos = Input.mousePosition;

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
    }

    private void OnAnimalRidden(float yPos)
    {
        Debug.Log("animal yPos: " + yPos);
        riddenYPos = (yPos * 2f) + transform.position.y;
        transform.position = new Vector3(transform.position.x, riddenYPos, transform.position.z);
        isRiding = true;
        // Disable player controls when riding an animal
        //enabled = false;
    }
}
