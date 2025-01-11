using UnityEngine;

public class ParticlePos : MonoBehaviour
{
    public Transform playerTransform; 

    void Update()
    {
        if (playerTransform != null)
        {
          
            transform.position = playerTransform.position;
        }
    }
}
