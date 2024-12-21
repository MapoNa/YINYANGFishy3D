using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private Rigidbody rb;
    // Start is called before the first frame update
    [SerializeField] float MoveSpeed;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        //HandleMovement();
        //HandleFishJump();
        rb.AddForce(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * MoveSpeed);
        print("?");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
