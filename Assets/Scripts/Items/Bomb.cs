using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField]
    private float explosionRadius = 5f;

    [SerializeField]
    private float explosionForce = 10f;


    [SerializeField]
    private float timeToExplode = 3f;

    [SerializeField]
    private LayerMask explosionLayer;

    private void Start()
    {
        InvokeRepeating("Explode", timeToExplode, timeToExplode);
    }

    public void Explode()
    {
        Collider[] colliders = new Collider[50]; // Create an array to store the colliders
        int numColliders =
            Physics.OverlapSphereNonAlloc(transform.position, explosionRadius,
                                          colliders, explosionLayer); // Get the colliders and store them in the array
        Debug.Log(numColliders);
        for (int i = 0; i < numColliders; i++)
        {
            if (colliders[i].GetComponent<Rigidbody>())
            {
                colliders[i].GetComponent<Rigidbody>()
                            .AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f,
                                               ForceMode.Impulse);
            }

            if (colliders[i].GetComponent<Wall>())
            {
                Destroy(colliders[i].gameObject);
            }

            if (colliders[i].GetComponent<BossAgent>())
            {
                var bossAgent = colliders[i].GetComponent<BossAgent>();
                bossAgent.TakeDamage(1);
            }
        }

        Destroy(transform.parent.gameObject);
    }
}