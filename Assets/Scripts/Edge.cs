using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{
    private Node node1;
    private Node node2;
    private Vector3 vector3;
    private bool bilateral;
    private int weight;

    public Edge(Node node1, Node node2, Vector3 vector3)
    {
        this.node1 = node1;
        this.node2 = node2;
        this.vector3 = vector3;
        this.bilateral = false;
    }

    public Node Node1
    {
        get => node1;
        set => node1 = value;
    }

    public Node Node2
    {
        get => node2;
        set => node2 = value;
    }

    public Vector3 Vector3
    {
        get => vector3;
    }

    public int Weight
    {
        get => weight;
        set => weight = value;
    }

    public void setVector3(float x, float y, float z)
    {
        vector3.x = x;
        vector3.y = y;
        vector3.z = z;
    }

    public bool isBilateral()
    {
        return bilateral;
    }

    public void setBilateral(bool isIt)
    {
        bilateral = isIt;
    }
}
