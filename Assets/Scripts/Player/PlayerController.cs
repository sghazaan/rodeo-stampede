using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotationAngle = 45f;
    [SerializeField] private float jumpForce = 8f;

    [Header("References")]
    [SerializeField] private JumpLandingIndicator jumpLandingIndicator;
    [SerializeField] private TextMeshProUGUI distanceText;
    public Rigidbody playerRigidbody;
    
    [Header("Colliders")]
    [SerializeField] private Collider triggerCollider; // Assign the trigger collider
    [SerializeField] private Collider physicsCollider; // Assign the non-trigger collider

    [Header("Position Settings")]
    [SerializeField] private float playerVerticalPosition = 0.5f;
    private Vector3 startPosition;
    private float distanceTraveled;

    [Header("Movement State")]
    private bool isJumping;
    private bool isRiding;
    private float riddenYPos;
    private Animal riddenAnimal;
    private Vector3 moveVelocity;

    [Header("Touch Controls")]
    private bool isHolding;
    private bool canJump;
    private bool isResettingRotation;
    private Vector2 touchStartPos;
    private Vector2 touchCurrentPos;

    private void OnEnable() => EventHub.OnAnimalRidden += OnAnimalRidden;
    private void OnDisable() => EventHub.OnAnimalRidden -= OnAnimalRidden;

    private void Start()
    {
        InitializePlayer();
    }

    private void InitializePlayer()
    {
        startPosition = transform.position;
        // if (playerRigidbody != null)
        // {
        //     playerRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        //     playerRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        //     playerRigidbody.freezeRotation = true;
        //     playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation | 
        //                                 RigidbodyConstraints.FreezePositionX;
        // }
        //moveVelocity = Vector3.forward * moveSpeed;
    }

    private void Update()
    {
        if (GameManager.IsGameOver) return;
        MovePlayer();
        HandleInput();
        UpdateDistanceUI();
    }

    private void MovePlayer()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        if (isRiding && !isJumping)
        {
            transform.position = new Vector3(transform.position.x, riddenYPos, transform.position.z);
        }
    }

    private void HandleInput()
    {
        HandleTouchStart();
        if (Input.GetMouseButton(0) && isHolding)
        {
            HandleDragging();
        }
        if (isHolding && !Input.GetMouseButton(0))
        {
            HandleJump();
        }
    }

    private void HandleTouchStart()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isHolding = true;
            touchStartPos = Input.mousePosition;
            if (!isJumping)
            {
                canJump = true;
            }
        }
    }

    private void HandleJump()
    {
        isHolding = false;
        if (!canJump || isJumping) return;
        Debug.Log("Jumping");
        playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0f, playerRigidbody.velocity.z);
        playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        // Apply forward force to boost movement
        //playerRigidbody.AddForce(transform.forward * (jumpForce * 0.5f), ForceMode.Impulse);
        isJumping = true;
        jumpLandingIndicator.StartJump();
        HandleDismount();
        canJump = false;
    }

    private void HandleDismount()
    {
        if (riddenAnimal == null) return;

        riddenAnimal.isRidden = false;
        riddenAnimal = null;
        isRiding = false;
        GameManager.IsPlayerRiding = false;
    }

    private void HandleDragging()
    {
        touchCurrentPos = Input.mousePosition;
        float dragDistance = touchCurrentPos.x - touchStartPos.x;
        
        if (Mathf.Abs(dragDistance) > 50)
        {
            float rotationAmount = Mathf.Clamp(dragDistance * 0.25f, -rotationAngle, rotationAngle);
             transform.rotation = Quaternion.Euler(0, rotationAmount, 0); // Rotate on Y-axis
            Debug.Log("Rotating: " + rotationAmount);
            touchStartPos = touchCurrentPos;
            // Stop any running reset coroutine to prevent interference
            StopCoroutine(nameof(ResetRotationSmoothly));
        }
        else if (!isResettingRotation)
        {
            StartCoroutine(ResetRotationSmoothly());
        }
    }

    private void UpdateDistanceUI()
    {
        distanceTraveled = transform.position.z - startPosition.z;
        if (distanceText != null)
        {
            distanceText.text = $"Distance: {Mathf.FloorToInt(distanceTraveled)}m";
        }
    }

    private IEnumerator ResetRotationSmoothly()
    {
        isResettingRotation = true;
        while (Mathf.Abs(transform.rotation.eulerAngles.y) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 5f);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 0, 0);
        isResettingRotation = false;
    }

    private void OnAnimalRidden(float yPos, Animal animal)
    {
        if (GameManager.IsPlayerRiding) return;

        riddenYPos = (yPos * 2f) + playerVerticalPosition;
        transform.position = new Vector3(transform.position.x, riddenYPos, transform.position.z);
        isRiding = true;
        GameManager.IsPlayerRiding = true;
        riddenAnimal = animal;
        
        // Disable physics collider when riding
        //if (physicsCollider != null) physicsCollider.enabled = false;

        // Disable player drag controls when riding an animal
        
        ResetJumpVariables();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            ResetJumpVariables();
            // Re-enable physics collider
            //if (physicsCollider != null) physicsCollider.enabled = true;
        }
    }

    public void ResetJumpVariables()
    {
        isJumping = false;
        canJump = true;
        jumpLandingIndicator.EndJump();
    }
}