using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody playerRigidbody; // Assign in Inspector
    public float jumpForce;      // Jump force to apply
    [SerializeField] private float moveSpeed;
    [SerializeField] private float dragSensitivity;
    [SerializeField] private float rotationAngle;
    [SerializeField] private JumpLandingIndicator jumpLandingIndicator;
     public TextMeshProUGUI distanceText; 
    
    private bool isHolding = false;   // Tracks if the user is holding down
    private bool canJump = false;     // Only allow jumping when user releases after holding
    private bool isJumping = false;   // Tracks if the player is currently in the air

    private Vector2 touchStartPos;    // Store the position where touch started
    private Vector2 touchCurrentPos;  // Store the current touch position

    float riddenYPos; // Y position of the player when riding an animal
    bool isRiding = false; // Whether the player is riding an animal
    private Animal riddenAnimal;

    private Vector3 startPosition;
    private float distanceTraveled = 0f;

    void OnEnable()
    {
        EventHub.OnAnimalRidden += OnAnimalRidden;
    }

    void OnDisable()
    {
        EventHub.OnAnimalRidden -= OnAnimalRidden;
    }

    void Start()
    {
        startPosition = transform.position; // Save the starting position
    }
    void Update()
    {
        if(GameManager.IsGameLost)
        {
            return;
        }
        // Move the player forward
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        if(isRiding && !isJumping)
        {
            transform.position = new Vector3(transform.position.x, riddenYPos, transform.position.z);
        }
        TouchStart();
        // Detect dragging
        if (Input.GetMouseButton(0) && isHolding)
        { 
            OnDragging();
        }
         // Detect touch release & trigger jump
        if (isHolding && !Input.GetMouseButton(0))
        {
            Jump();
        }
        distanceTraveled = transform.position.z - startPosition.z;
        if (distanceText != null)
        {
            distanceText.text = "Distance: " + Mathf.FloorToInt(distanceTraveled) + "m"; 
        }

        
    }

    private void TouchStart()
    {
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
    }

    private void Jump()
    {
        isHolding = false;
        // Jump only if we were allowed to jump before release
        if (canJump && !isJumping)
        {
            Debug.Log("Jumping");
            playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0f, playerRigidbody.velocity.z);
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            // Apply forward force to boost movement
            playerRigidbody.AddForce(transform.forward * (jumpForce * 0.5f), ForceMode.Impulse);
            isJumping = true;  // Set jumping state
            jumpLandingIndicator.StartJump(); 
            if(riddenAnimal != null)
            {
                riddenAnimal.isRidden = false;
                riddenAnimal = null;
                isRiding = false;
                GameManager.IsPlayerRiding = false;
            }
        }
        // Reset jump permission so it doesn't trigger multiple times
        canJump = false;
    }

    void OnDragging()
    {
        touchCurrentPos = Input.mousePosition;
        float dragDistance = touchCurrentPos.x - touchStartPos.x;
        if (Mathf.Abs(dragDistance) > 50) // Adjust threshold for dragging
        {
            float rotationAmount = Mathf.Clamp(dragDistance * 0.25f, -rotationAngle, rotationAngle); // Limit rotation range
            transform.rotation = Quaternion.Euler(0, rotationAmount, 0); // Rotate on Y-axis
            Debug.Log("Rotating: " + rotationAmount);
            touchStartPos = touchCurrentPos;
            // Stop any running reset coroutine to prevent interference
            StopCoroutine(nameof(ResetRotationSmoothly));
        }
        else if (!IsInvoking(nameof(ResetRotationSmoothly)))
        {
            // Start the reset coroutine if it's not already running
            StartCoroutine(ResetRotationSmoothly());
        }
    }

    // Coroutine to smoothly reset rotation back to zero
    private IEnumerator ResetRotationSmoothly()
    {
        while (Quaternion.Angle(transform.rotation, Quaternion.identity) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * 5f);
            yield return null;
        }

        transform.rotation = Quaternion.identity; // Ensure it's fully reset
    }

    private void OnAnimalRidden(float yPos, Animal animal)
    {
        if(GameManager.IsPlayerRiding)
        {
            return;
        }
        Debug.Log("animal yPos: " + yPos);
        riddenYPos = (yPos * 2f) + transform.position.y;
        transform.position = new Vector3(transform.position.x, riddenYPos, transform.position.z);
        isRiding = true;
        GameManager.IsPlayerRiding = true;
        riddenAnimal = animal;
        // Disable player controls when riding an animal
        //enabled = false;
    }

     // Detect ground collision
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            isJumping = false;  // Allow jumping again when landed
            jumpLandingIndicator.EndJump();
        }
    }
}
