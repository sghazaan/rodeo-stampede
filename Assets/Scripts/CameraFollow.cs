using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Assign the player transform in the inspector
    public Vector3 offset = new Vector3(0, 5, -10); // Camera offset
    public float smoothSpeed = 0.125f; // Smoothing factor

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(player); // Optional: Keep the camera focused on the player
    }
}