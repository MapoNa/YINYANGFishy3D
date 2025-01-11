using UnityEngine;

public class RopeEndPointController : MonoBehaviour
{
    public Transform startPoint; // The starting point of the rope
    public float maxDistance = 2f; // The maximum allowed distance between the start and end points

    void Update()
    {
        if (startPoint == null)
        {
            Debug.LogWarning("Start point is not assigned!");
            return;
        }

        // Calculate the current distance between the end point and the start point
        float currentDistance = Vector3.Distance(transform.position, startPoint.position);

        // If the distance exceeds the maximum allowed value, pull the end point back within the limit
        if (currentDistance > maxDistance)
        {
            Vector3 direction = (transform.position - startPoint.position).normalized;
            transform.position = startPoint.position + direction * maxDistance;
        }
    }
}
