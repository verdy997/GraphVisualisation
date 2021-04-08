using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeData : MonoBehaviour
{
    private List<NodeData> iNodes = new List<NodeData>();
    private List<EdgeData> iEdges = new List<EdgeData>();
    private int id;
    private Vector3 vector3;
    private Vector3 forceVector3;
    private string region;
    private int degree;
    private bool cluster;

    private void Start()
    {
        this.name = id.ToString();
    }

    public int ID
    {
        get => id;
        set => id = value;
    }

    public Vector3 Vector3
    {
        get => vector3;
        set => vector3 = value;
    }

    public Vector3 ForceVector3
    {
        get => forceVector3;
        set => forceVector3 = value;
    }

    public string Region
    {
        get => region;
        set => region = value;
    }

    public int Degree
    {
        get => degree;
        set => degree = value;
    }

    public bool Cluster
    {
        get => cluster;
        set => cluster = value;
    }

    public List<NodeData> getiNodes()
    {
        return iNodes;
    }
    
    public List<EdgeData> getiEdges()
    {
        return iEdges;
    }
    
    public void AddiNode(NodeData node)
    {
        iNodes.Add(node);
    }
    
    public void AddiEdge(EdgeData edge)
    {
        iEdges.Add(edge);
    }
    
    public void setVector3(float x, float y, float z)
    {
        vector3.x = x;
        vector3.y = y;
        vector3.z = z;
    }
    
    public void setForceVector3(float x, float y, float z)
    {
        forceVector3.x = x;
        forceVector3.y = y;
        forceVector3.z = z;
    }
}
