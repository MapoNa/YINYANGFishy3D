using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EatingFishManager : MonoBehaviour
{
    private FishController fishController;
    private FixedJoint joint;

    [SerializeField]
    private float eatingSize = 0.5f;

    [SerializeField]
    private LayerMask eatableLayer;

    [SerializeField]
    private Transform fishMouseTransform;

    private Rigidbody redBallRb;

    private void Start()
    {
        fishController = GetComponent<FishController>();
        joint = fishMouseTransform.GetComponent<FixedJoint>();
    }

    private void Update()
    {
        if (fishController.isEating && transform.localScale.x >= eatingSize && joint.connectedBody == null)
        {
            foreach (var col in GetColliders())
            {
                if (col.GetComponent<Rigidbody>())
                {
                    redBallRb = col.GetComponent<Rigidbody>();
                    if (transform.localScale.x < redBallRb.transform.localScale.x)
                        continue;
                    redBallRb.gameObject.transform.position = fishMouseTransform.position + transform.forward * 0.1f;
                    redBallRb.transform.parent = fishMouseTransform.transform;
                    SetBallToPlayer(redBallRb);
                    Debug.Log("Finding a red ball" + redBallRb.gameObject.name);
                    break;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (joint.connectedBody != null)
            {
                RemoveBallFromPlayer();
            }

            foreach (var col in GetColliders())
            {
                if (col.GetComponent<IEatable>() != null)
                {
                    col.GetComponent<IEatable>().CheckEatable(transform.localScale);
                    break;
                }
            }
        }
    }


    private void SetBallToPlayer(Rigidbody rb)
    {
        joint.connectedBody = rb;
    }

    private void RemoveBallFromPlayer()
    {
        redBallRb.transform.parent = null;
        joint.connectedBody = null;
    }

    private List<Collider> GetColliders()
    {
        var coliders = Physics.OverlapSphere(fishMouseTransform.position, 0.1f, eatableLayer);

        return new List<Collider>(coliders);
    }
}