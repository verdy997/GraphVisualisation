using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Subgraph : MonoBehaviour
{
    private List<NodeData> nodes = new List<NodeData>();
    private List<EdgeData> edges = new List<EdgeData>();
    private int id;
    private string region;

    private void Start()
    {
        this.name = region;
    }

    public int ID
    {
        get => id;
        set => id = value;
    }

    public string Region
    {
        get => region;
        set => region = value;
    }

    public List<NodeData> getNodes()
    {
        return nodes;
    }

    public List<EdgeData> getEdges()
    {
        return edges;
    }

    public void Active(bool active)
    {
        foreach (var node in nodes)
        {
            node.gameObject.SetActive(active);
        }

        foreach (var edge in edges)
        {
            edge.gameObject.SetActive(active);
        }
    }

    public bool ContainsId(int id)
    {
        int tmp = nodes.Where(n => n.ID == id).Count();
        if (tmp == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
