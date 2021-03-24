using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    private List<Node> nodes { get; set; }
    private List<Edge> edges { get; set; }

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

    public List<Node> getNodes()
    {
        return nodes;
    }
    
    public List<Edge> getEdges()
    {
        return edges;
    }
    
}
