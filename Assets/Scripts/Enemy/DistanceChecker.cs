using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceChecker : MonoBehaviour
{
    public float DistanceFromPlayer()
    {
        return Vector3.Distance(this.transform.position, PlayerSingleton.Instance.transform.position);
    }

    public Vector3 DirectionFromPlayer()
    {
        return (PlayerSingleton.Instance.transform.position - this.transform.position).normalized;
    }
}