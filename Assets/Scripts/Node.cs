using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    private List<Node> iNodes { get; set;}
    private List<Edge> iEdges { get; set;}
    private int id;
    private Vector3 vector3;
    private Vector3 forceVector3;
    private string region;
    private int degree;

    public Node(int id, Vector3 vector3, string region)
    {
        this.id = id;
        this.vector3 = vector3;
        this.iNodes = new List<Node>();
        this.iEdges = new List<Edge>();
        this.region = region;
    }

    public int ID
    {
        get => id;
        set => id = value;
    }

    public Vector3 Vector3
    {
        get => vector3;
    }

    public Vector3 ForceVector3
    {
        get => forceVector3;
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
    
    public void AddiNode(Node node)
    {
        iNodes.Add(node);
    }
    
    public List<Node> getiNodes()
    {
        return iNodes;
    }
    
    public void AddiEdge(Edge edge)
    {
        iEdges.Add(edge);
    }
    
    public List<Edge> getiEdges()
    {
        return iEdges;
    }
    
    public Node getiNodeByID(int searchingID)
    {
        for (int i = 0; i < iNodes.Capacity; i++)
        {
            if (iNodes[i].ID == searchingID)
            {
                return iNodes[i];
            }
        }
        return null;
    }
}
