using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronMove : MonoBehaviour
{
    Rigidbody rb;
    
    void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        if (Input.GetKey(KeyCode.D)) 
        {
            rb.AddRelativeForce(new Vector3(2, 0, 0)); 
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddRelativeForce(new Vector3(-2, 0, 0));
        }
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddRelativeForce(new Vector3(0, 0, 2));
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddRelativeForce(new Vector3(0, 0, -2));
        }
        if (Input.GetKey(KeyCode.Q))
        {
            rb.AddRelativeForce(new Vector3(0, -2, 0));
        }
        if (Input.GetKey(KeyCode.E))
        {
            rb.AddRelativeForce(new Vector3(0, 2, 0));
        }
    }
}