using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
    public int LockID = 0;

    // Check if the Lock has the same ID as the key
    public void CheckKey(int id)
    {
        if (id == LockID)
        {
            Destroy(gameObject);
        }
    }
}