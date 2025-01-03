using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceChecker : MonoBehaviour
{
    public float DistanceFromPlayer()
    {
        // Calculate the distance between the current object's position and the player's position
        return Vector3.Distance(this.transform.position, PlayerSingleton.Instance.transform.position);
    }

    public Vector3 DirectionFromPlayer()
    {
        // Calculate the direction from the current object's position to the player's position
        return (PlayerSingleton.Instance.transform.position - this.transform.position).normalized;
    }
}