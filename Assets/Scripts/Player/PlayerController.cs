using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float dragSensitivity = 20f;
    [SerializeField] private float maxRotationAngle = 45f;
    [SerializeField] private float rotationResetDelay = 1f;
    [SerializeField] private float jumpForce = 8f;

    [Header("References")]
    [SerializeField] private JumpLandingIndicator jumpLandingIndicator;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI finalDistanceText;
    public Rigidbody playerRigidbody;
    [SerializeField] private PlayerAnimationController playerAnimationController;
    
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

    private float originalMoveSpeed;
    

    private void OnEnable() => EventHub.OnAnimalRidden += OnAnimalRidden;
    private void OnDisable() => EventHub.OnAnimalRidden -= OnAnimalRidden;

    private void Start()
    {
        originalMoveSpeed = moveSpeed;
        InitializePlayer();
    }

    private void InitializePlayer()
    {
        startPosition = transform.position;
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
        Vector3 newPosition = transform.position + (transform.forward * moveSpeed * Time.deltaTime);
        newPosition.x = Mathf.Clamp(newPosition.x, -4.5f, 4.5f);
        if (isRiding && !isJumping)
        {
            newPosition.y = riddenYPos;
        }
        playerRigidbody.MovePosition(newPosition);
    } 

    private void HandleDragging()
    {
        touchCurrentPos = Input.mousePosition;
        float dragDistance = touchCurrentPos.x - touchStartPos.x;
        
        if (Mathf.Abs(dragDistance) > dragSensitivity)
        {
           // StopCoroutine(ResetRotationSmoothly());
            float rotationAmount = Mathf.Clamp(dragDistance/4f, -maxRotationAngle, maxRotationAngle);
            Debug.Log($"Rotation Amount: {rotationAmount}");
            
            // Get current rotation and clamp it
           // float currentYRotation = transform.rotation.eulerAngles.y;
            //Debug.Log($"Current Rotation: {currentYRotation}");
            float clampedRotation = Mathf.Clamp( rotationAmount, -maxRotationAngle, maxRotationAngle);
            Debug.Log($"Clamped Rotation: {clampedRotation}");
            playerRigidbody.MoveRotation(Quaternion.Euler(0, clampedRotation, 0));
            touchStartPos = touchCurrentPos;
        }
        // else if(!isResettingRotation)
        // {
        //     StartCoroutine(ResetRotationSmoothly());
        // }
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
        playerAnimationController.SetIsRunning(false);
        playerAnimationController.SetIsRiding(false);
        playerAnimationController.SetIsJumping(true);
    }

    private void HandleDismount()
    {
        if (riddenAnimal == null) return;

        riddenAnimal.isRidden = false;
        riddenAnimal = null;
        isRiding = false;
        GameManager.IsPlayerRiding = false;
    }

    private IEnumerator ResetRotationSmoothly()
    {
        isResettingRotation = true;
        yield return new WaitForSeconds(rotationResetDelay);
        while (Mathf.Abs(transform.rotation.eulerAngles.y) > 0.1f)
        {
            Quaternion targetRotation = Quaternion.Euler(0, 0, 0);
            Quaternion newRotation = Quaternion.Slerp(playerRigidbody.rotation, targetRotation, Time.deltaTime * 5f);
            playerRigidbody.MoveRotation(newRotation);
            yield return null;
        }
        playerRigidbody.MoveRotation(Quaternion.Euler(0, 0, 0));
        isResettingRotation = false;
    }

    private void UpdateDistanceUI()
    {
        distanceTraveled = transform.position.z - startPosition.z;
        if (distanceText != null)
        {
            distanceText.text = $"{Mathf.FloorToInt(distanceTraveled)}m";
            finalDistanceText.text = "Total Distnce: " + $"{Mathf.FloorToInt(distanceTraveled)}m";
        }
    }

    

    private void OnAnimalRidden(float yPos, Animal animal)
    {
        if (GameManager.IsPlayerRiding) return;
        playerAnimationController.SetIsJumping(false);
        playerAnimationController.SetIsRunning(false);
        playerAnimationController.SetIsRiding(true);
        
        riddenYPos = yPos + playerVerticalPosition;
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
        }
        else if(collision.gameObject.CompareTag("Hurdle"))
        {
            GameHandler.Instance.GameOver();
        }
    }

    public void ResetJumpVariables()
    {
        isJumping = false;
        canJump = true;
        jumpLandingIndicator.EndJump();  
        if(!GameManager.IsPlayerRiding && !isJumping)
        {
            SetIsRunning(true);
        }
    }

    public void SetIsRunning(bool isRunning)
    {
        playerAnimationController.SetIsRunning(isRunning);
        playerAnimationController.SetIsJumping(false);
        playerAnimationController.SetIsRiding(false);
    }

    public void GameOverAnimation()
    {
        isRiding = false;
        isJumping = false;
        playerAnimationController.SetIsJumping(false);
        playerAnimationController.SetIsRiding(false);
        playerAnimationController.SetIsRunning(false);
        playerAnimationController.SetIsDead(true);
    }
}