using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    //public int keyID = 0;
    public int keyID = 0;

    private void OnCollisionEnter(Collision other)
    {
        // Check if the object that entered the trigger is a Lock
        if (other.gameObject.GetComponent<Lock>())
        {
            // Get the Lock component of the object
            var lockObj = other.gameObject.GetComponent<Lock>();
            // Check if the Lock has the same ID as the key
            if (lockObj.LockID == keyID)
            {
                lockObj.CheckKey(keyID);
                Destroy(gameObject);
            }
        }
    }
}