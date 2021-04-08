using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeData : MonoBehaviour
{
    private NodeData node1;
    private NodeData node2;
    private Vector3 vector3;
    private bool bilateral;
    private int weight;

    private void Start()
    {
        this.name = node1.ID + "-" + node2.ID;
    }
    
    public void UpdatePosition()
    {
        float distance = Vector3.Distance(node1.Vector3, node2.Vector3);
        Vector3 newZ = this.transform.localScale;
        newZ.z = distance / 2;
        this.transform.localScale = newZ;
        this.transform.position = node1.Vector3;
        this.transform.LookAt(node2.Vector3);
    }

    public NodeData Node1
    {
        get => node1;
        set => node1 = value;
    }

    public NodeData Node2
    {
        get => node2;
        set => node2 = value;
    }

    public Vector3 Vector3
    {
        get => vector3;
        set => vector3 = value;
    }

    public bool Bilateral
    {
        get => bilateral;
        set => bilateral = value;
    }

    public int Weight
    {
        get => weight;
        set => weight = value;
    }
}
