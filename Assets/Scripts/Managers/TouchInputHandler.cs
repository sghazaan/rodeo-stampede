using UnityEngine;

public class TouchInputHandler : MonoBehaviour
{
    private bool isHolding = false; // To track if the user is holding down
    private Vector2 touchStartPos; // To store the position where touch started
    private Vector2 touchCurrentPos; // To store the current touch position

    void Update()
    {
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
                    Debug.Log("Dragging Right");
                }
                else
                {
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
        }
    }
}
