using System;
using System.Collections;
using System.Collections.Generic;
using GogoGaga.OptimizedRopesAndCables;
using UnityEngine;

public class Saizi : MonoBehaviour
{
    //get the rigidbody
    public Rigidbody ConnectBody;

    // get the rope
    public Rope RopePrefab;

    private void OnTriggerEnter(Collider other)
    {
        //check if the object that entered the trigger is a SaiziBot
        if (other.gameObject.transform.parent.GetComponent<SaiziBot>())
        {
            //get the saizi bot
            var saiziBot = other.gameObject.transform.parent.GetComponent<SaiziBot>();
            //connect the saizi bot
            ConnectBody.MovePosition(saiziBot.transform.position);
            saiziBot.Joint.connectedBody = ConnectBody;
            RopePrefab.EndPoint = saiziBot.transform;
        }
    }
}