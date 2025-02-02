using System.Collections.Generic;
using UnityEngine;

public class JumpLandingIndicator : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float forwardOffset = 0.5f;
    [SerializeField] private float radius = 1f;  // Radius of the yellow circle
    private Renderer circleRenderer;
    private bool isJumping = false;
    private HashSet<Renderer> highlightedAnimals = new HashSet<Renderer>(); // Track colored animals
    private Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>(); // Store original colors
    private Renderer animalRenderer;
    HashSet<Renderer> currentAnimals;
    Collider[] overlappedColliders;
    void Start()
    {
        circleRenderer = GetComponent<Renderer>();
        circleRenderer.enabled = false; // Hide the circle initially
    }

    void Update()
    {
        if (!isJumping) return;

        // Update position based on player's movement
        transform.position = new Vector3(player.position.x, 0f, player.position.z + forwardOffset);

        // Check for animals inside the circle
        overlappedColliders = Physics.OverlapSphere(transform.position, radius);
        currentAnimals = new HashSet<Renderer>();
        foreach (Collider col in overlappedColliders)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Animal"))
            {
                animalRenderer = col.GetComponent<Renderer>();
                currentAnimals.Add(animalRenderer); 

                // Change color if not already changed
                if (!highlightedAnimals.Contains(animalRenderer))
                {
                    originalColors[animalRenderer] = animalRenderer.material.color;
                    animalRenderer.material.color = Color.yellow;
                    highlightedAnimals.Add(animalRenderer);
                }

                // If user taps while jumping & an animal is inside the circle, land on the animal
                if (Input.GetMouseButtonDown(0))
                {
                    LandOnAnimal(col.transform);
                    return;
                }
            }
        }

        // Restore color for animals that left the circle
        RestoreAnimalColors(currentAnimals);
    }

    public void StartJump()
    {
        isJumping = true;
        circleRenderer.enabled = true;
    }

    public void EndJump()
    {
        isJumping = false;
        circleRenderer.enabled = false;
    }

    private void LandOnAnimal(Transform animal)
    {
        isJumping = false;
        circleRenderer.enabled = false;

        // Move player to animal's position (land on the animal)
        player.position = new Vector3(animal.position.x, animal.position.y + 0.5f, animal.position.z);

         // Reset animal colors
        RestoreAnimalColors(new HashSet<Renderer>());
    }

    private void RestoreAnimalColors(HashSet<Renderer> currentAnimals)
    {
        List<Renderer> toRemove = new List<Renderer>();

        foreach (Renderer animalRenderer in highlightedAnimals)
        {
            if (!currentAnimals.Contains(animalRenderer)) // If no longer in circle
            {
                animalRenderer.material.color = originalColors[animalRenderer]; // Restore original color
                toRemove.Add(animalRenderer);
            }
        }

        // Remove restored animals from tracking lists
        foreach (Renderer animalRenderer in toRemove)
        {
            highlightedAnimals.Remove(animalRenderer);
            originalColors.Remove(animalRenderer);
        }
    }
}
