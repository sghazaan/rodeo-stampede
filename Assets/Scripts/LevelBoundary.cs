using UnityEngine;

public class LevelBoundary : MonoBehaviour
{
    // Define the boundaries for the level
    private float minX = -4.5f;
    private float maxX = 4.5f;
   

    void Update()
    {
        // Clamp the object's position within the boundaries
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        transform.position = clampedPosition;
    }
}
