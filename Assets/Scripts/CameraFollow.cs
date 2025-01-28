using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Assign the player transform in the inspector
    public Vector3 offset = new Vector3(0, 0, 0);//camera offset
    public float smoothSpeed = 0.125f; // Smoothing factor
    #region Local variables
    Vector3 desiredPosition, smoothedPosition;
    #endregion

    void LateUpdate()
    {
        if (player == null) return;
        desiredPosition = player.position + transform.rotation * offset;
        smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        //ransform.LookAt(player); 
    }
}