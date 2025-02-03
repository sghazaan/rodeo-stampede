using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpLandingIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Material indicatorMaterial;

    [Header("Landing Settings")]
    [SerializeField] private float initialJumpOffset = 3f;
    [SerializeField] private float minJumpOffset = 0.5f;
    [SerializeField] private float radius = 1f;
    [SerializeField] private float landingDuration = 0.3f;
    [SerializeField] private float landingHeight = 1f;
    [SerializeField] private float jumpProgressSpeed = 1f;
    [SerializeField] private LayerMask groundLayer; // Add layer mask for ground detection

    [Header("Visual Settings")]
    private Renderer circleRenderer;
    private bool isJumping = false;
    private bool isLanding = false;
    private float landingTimer = 0f;
    private float jumpProgress = 0f;
    private Vector3 landingStartPos;
    private Vector3 landingTargetPos;
    private Transform targetAnimal;
    private bool landingOnAnimal = false;

    private HashSet<Renderer> highlightedAnimals = new HashSet<Renderer>();
    private Dictionary<Renderer, List<Material>> originalMaterials = new Dictionary<Renderer, List<Material>>();
    private HashSet<Renderer> currentAnimals = new HashSet<Renderer>();

    void Start()
    {
        circleRenderer = GetComponent<Renderer>();
        circleRenderer.enabled = false;
    }

    void Update()
    {
        if (!isJumping && !isLanding) return;

        if (isLanding)
        {
            HandleLandingAnimation();
            return;
        }

        if (isJumping)
        {
            jumpProgress += Time.deltaTime * jumpProgressSpeed;
            float progressFactor = Mathf.Clamp01(jumpProgress);
            
            float currentOffset = Mathf.Lerp(initialJumpOffset, minJumpOffset, Mathf.SmoothStep(0f, 1f, progressFactor));
            Vector3 forward = player.forward;
            Vector3 targetPosition = player.position + (forward * currentOffset);
            transform.position = new Vector3(targetPosition.x, 0f, targetPosition.z);

            CheckForLanding();
            UpdateAnimalMaterials(); // Update animal materials based on overlap
        }
    }

    private void CheckForLanding()
    {
        // First check for animals
        bool foundAnimal = CheckForAnimals();

        // If no animal was clicked and player clicks, try landing on ground
        if (!foundAnimal && Input.GetMouseButtonDown(0))
        {
            // Cast ray down from indicator position to check ground
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 10f, Vector3.down, out hit, 20f, groundLayer))
            {
                StartLandingOnGround(hit.point);
            }
        }
    }
    private bool CheckForAnimals()
    {
        Collider[] overlappedColliders = Physics.OverlapSphere(transform.position, radius);
        currentAnimals.Clear();
        bool foundClickableAnimal = false;

        foreach (Collider col in overlappedColliders)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Animal"))
            {
                Renderer animalRenderer = col.transform.GetChild(0).GetComponent<Renderer>();
                if (animalRenderer != null)
                {
                    currentAnimals.Add(animalRenderer);

                    if (!highlightedAnimals.Contains(animalRenderer))
                    {
                        if(animalRenderer.materials.Length > 1)
                        {
                            List<Material> materials = new List<Material>();
                            foreach(Material mat in animalRenderer.materials)
                            {
                                materials.Add(mat);
                            }
                            originalMaterials[animalRenderer] = materials;
                            Material[] newMaterials = new Material[materials.Count];
                            for(int i = 0; i < materials.Count; i++)
                            {
                                newMaterials[i] = indicatorMaterial;
                            }
                            animalRenderer.materials = newMaterials;
                            highlightedAnimals.Add(animalRenderer);
                        }
                        else
                        {
                            List<Material> materials = new List<Material>();
                            materials.Add(animalRenderer.material);
                            originalMaterials[animalRenderer] = materials;
                            animalRenderer.material = indicatorMaterial;
                            highlightedAnimals.Add(animalRenderer);
                        }
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        StartLandingOnAnimal(col.transform);
                        foundClickableAnimal = true;
                        break;
                    }
                }
            }
        }

        RestoreAnimalMaterials(currentAnimals);
        return foundClickableAnimal;
    }

    private void UpdateAnimalMaterials()
    {
        // Restore materials for animals that are no longer overlapping
        RestoreAnimalMaterials(currentAnimals);
    }

    private void StartLandingOnGround(Vector3 landingPoint)
    {
        isLanding = true;
        landingOnAnimal = false;
        landingTimer = 0f;
        landingStartPos = player.position;
        landingTargetPos = new Vector3(
            landingPoint.x,
            landingPoint.y,
            landingPoint.z
        );
    }

    private void StartLandingOnAnimal(Transform animal)
    {
        isLanding = true;
        landingOnAnimal = true;
        targetAnimal = animal;
        landingTimer = 0f;
        landingStartPos = player.position;
        landingTargetPos = new Vector3(
            animal.position.x,
            animal.position.y + landingHeight,
            animal.position.z
        );
    }

    private void HandleLandingAnimation()
    {
        landingTimer += Time.deltaTime;
        float progress = landingTimer / landingDuration;

        if (progress >= 1f)
        {
            CompleteLanding();
            return;
        }

        float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);
        player.position = Vector3.Lerp(landingStartPos, landingTargetPos, smoothProgress);
    }

    private void CompleteLanding()
    {
        isLanding = false;
        isJumping = false;
        circleRenderer.enabled = false;
        
        player.position = landingTargetPos;
        RestoreAnimalMaterials(new HashSet<Renderer>());

        if (playerController != null)
        {
            playerController.ResetJumpVariables();
        }

        if (landingOnAnimal && targetAnimal != null)
        {
            Animal animal = targetAnimal.GetComponent<Animal>();
            if (animal != null)
            {
                animal.isRidden = true;
                EventHub.InvokeAnimalRidden(targetAnimal.position.y, animal);
            }
        }
    }

    public void StartJump()
    {
        gameObject.SetActive(true);
        isJumping = true;
        isLanding = false;
        circleRenderer.enabled = true;
        landingTimer = 0f;
        jumpProgress = 0f;
        landingOnAnimal = false;
    }

    public void EndJump()
    {
        if (!isLanding)
        {
            isJumping = false;
            circleRenderer.enabled = false;
            RestoreAnimalMaterials(new HashSet<Renderer>());
        }
    }

    private void RestoreAnimalMaterials(HashSet<Renderer> currentAnimals)
    {
        List<Renderer> toRemove = new List<Renderer>();

        foreach (Renderer animalRenderer in highlightedAnimals)
        {
            if (!currentAnimals.Contains(animalRenderer))
            {
                if (originalMaterials.ContainsKey(animalRenderer))
                {
                    if(animalRenderer.materials.Length > 1)
                    {
                        Material[] newMaterials = new Material[originalMaterials[animalRenderer].Count];
                        for(int i = 0; i < originalMaterials[animalRenderer].Count; i++)
                        {
                            newMaterials[i] = originalMaterials[animalRenderer][i];
                        }
                        animalRenderer.materials = newMaterials;
                    }
                    else
                    {
                        animalRenderer.material = originalMaterials[animalRenderer][0];
                    }
                    toRemove.Add(animalRenderer);
                }
            }
        }

        foreach (Renderer animalRenderer in toRemove)
        {
            highlightedAnimals.Remove(animalRenderer);
            originalMaterials.Remove(animalRenderer);
        }
    }
}