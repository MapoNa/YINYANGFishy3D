using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public int keyID = 0;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<Lock>())
        {
            var lockObj = other.gameObject.GetComponent<Lock>();
            {
                
            }
            if (lockObj.LockID == keyID)
            {
                lockObj.CheckKey(keyID);
                Destroy(gameObject);
            }
        }
    }
}