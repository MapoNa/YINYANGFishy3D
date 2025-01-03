using System;
using System.Collections;
using System.Collections.Generic;
using GogoGaga.OptimizedRopesAndCables;
using UnityEngine;

public class Saizi : MonoBehaviour
{
    public Rigidbody ConnectBody;

    public Rope RopePrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.parent.GetComponent<SaiziBot>())
        {
            var saiziBot = other.gameObject.transform.parent.GetComponent<SaiziBot>();
            ConnectBody.position = saiziBot.transform.position;
            saiziBot.Joint.connectedBody = ConnectBody;
            RopePrefab.EndPoint = saiziBot.transform;
        }
    }
}