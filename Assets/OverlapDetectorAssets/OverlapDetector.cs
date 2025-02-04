using UnityEngine;

public class OverlapDetector : MonoBehaviour
{
    public GameObject sphereObject;
    public GameObject cuboidObject;
    #region private variables
    float centerCoord, minCoord, maxCoord, distanceSquared, sphereRadius, radiusSquared, diff;
    Vector3 cuboidMin, cuboidMax;
    #endregion

    private void Update()
    {
        // Check for overlap when space key is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool isOverlapping = CheckBoxSphereIntersection(sphereObject, cuboidObject);
            Debug.Log($"Intersection Result: {isOverlapping}");
        }
    }

    public bool CheckBoxSphereIntersection(GameObject sphere, GameObject cuboid)
    {
        Vector3 sphereCenter = sphere.transform.position;
        //GetSpehereDiameter gets max scale and multiplies radius with it
        sphereRadius = GetWorldSphereDiameter(sphere) / 2f;
        radiusSquared = sphereRadius * sphereRadius;

        // Get cube's min and max ranges of its faces
        cuboidMin = GetWorldMinExtents(cuboid);
        cuboidMax = GetWorldMaxExtents(cuboid);

        distanceSquared = 0f;

        // iterate for 3 axes of sphere and 6 axes of cube
        for (int i = 0; i < 3; i++)
        {
            centerCoord = sphereCenter[i];
            minCoord = cuboidMin[i];
            maxCoord = cuboidMax[i];

            // Check if sphere center is outside the box on this axis
            if (centerCoord < minCoord)
            {
                diff = centerCoord - minCoord;
                distanceSquared += diff * diff;
            }
            else if (centerCoord > maxCoord)
            {
                diff = centerCoord - maxCoord;
                distanceSquared += diff * diff;
            }
        }

        // if squared distance is less than or equal to sphere radius squared means they intersect
        return distanceSquared <= radiusSquared;
    }

    private float GetWorldSphereDiameter(GameObject sphere)
    {
        SphereCollider sphereCollider = sphere.GetComponent<SphereCollider>();
        Vector3 scale = sphere.transform.localScale;
        float maxScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
        return sphereCollider.radius * 2f * maxScale;
    }

    private Vector3 GetWorldMinExtents(GameObject cuboid)
    {
        BoxCollider boxCollider = cuboid.GetComponent<BoxCollider>();
        Vector3 localMin = boxCollider.center - (boxCollider.size / 2f);
        return cuboid.transform.TransformPoint(localMin);
    }

    private Vector3 GetWorldMaxExtents(GameObject cuboid)
    {
        BoxCollider boxCollider = cuboid.GetComponent<BoxCollider>();
        Vector3 localMax = boxCollider.center + (boxCollider.size / 2f);
        return cuboid.transform.TransformPoint(localMax);
    }
}