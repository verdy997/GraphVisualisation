using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    private List<Node> nodes { get; set; }
    private List<Edge> edges { get; set; }
    private List<int> freePositions { get; set; }

    public Graph()
    {
        this.nodes = new List<Node>();
        this.edges = new List<Edge>();
    }

    public void AddNode(int position, Node node)
    {
        nodes[position] = node;
    }
    
    public void AddNode(Node node)
    {
        nodes.Add(node);
    }

    public void AddEdge(Edge edge)
    {
        edges.Add(edge);
    }

    public void AddPosition(int position)
    {
        freePositions.Add(position);
    }

    public List<Node> getNodes()
    {
        return nodes;
    }
    
    public List<Edge> getEdges()
    {
        return edges;
    }

    public List<int> getFreePositions()
    {
        return freePositions;
    }

    public void DeleteNode(int position)
    {
        DeleteEdges(position);
        nodes[position] = null;
        if (!freePositions.Contains(position))
        {
            freePositions.Add(position);
        }
    }

    public void DeleteEdges(int idNode)
    {
        for (int i = 0; i <= edges.Count; i++)
        {
            if (edges[i].Node1.ID == idNode || edges[i].Node2.ID == idNode)
            {
                edges.RemoveAt(i);
            }
        }
    }

    public int? findFreePosition()
    {
        if (freePositions == null || freePositions.Count == 0)
        {
            return null;
        }
        return freePositions[0];
    }
    
}
