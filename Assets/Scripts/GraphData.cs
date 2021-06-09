using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

public class GraphData : MonoBehaviour
{
    [SerializeField] private NodeData node;
    [SerializeField] private EdgeData edge;
    [SerializeField] private EdgeData bilateral;
    [SerializeField] private RegionGraph regionGraph;
    
    private List<NodeData> nodes = new List<NodeData>();
    private List<EdgeData> edges = new List<EdgeData>();
    private bool active = false;
    private bool activeEdges;
    
    private void Start()
    {
        name = "Graph";
    }

    public List<NodeData> getNodes()
    {
        return nodes;
    }

    public List<EdgeData> getEdges()
    {
        return edges;
    }

    public void setRange(int fromN, int toN, List<string[]> linesN, List<int[]> linesE)
    {
        FillNodes((linesN.Where(nline => int.Parse(nline[0]) >= fromN && int.Parse(nline[0]) <= toN)).ToList());
        
        FillEdges((linesE.Where(eline => eline[0] >= fromN && eline[0] <= toN &&
                                             eline[1] >= fromN && eline[1] <= toN)).ToList(), fromN);
    }

    public void FillNodes(List<string[]> lines)
    {
        if (nodes.Count > 0)
        {
            RemoveAll();
            regionGraph.RemoveAll();
        }
        foreach (var line in lines)
        {
            float x;
            float y;
            float z;
            if (line.Length != 8)
            {
                Random rnd = new Random(Guid.NewGuid().GetHashCode());
                x = rnd.Next(0, 20000) / 100f;
                y = rnd.Next(0, 20000) / 100f;
                z = rnd.Next(0, 20000) / 100f;
            }
            else
            {
                x = float.Parse(line[1]);
                y = float.Parse(line[2]);
                z = float.Parse(line[5]);
            }
            
            NodeData n = CreateNode(int.Parse(line[0]), line[3], line[4], line[7], x, y, z); 
            nodes.Add(n);
            //AddNode(n);
        }
    }

    public NodeData CreateNode(int id, string gender, string region, string age, float x, float y, float z)
    {
        Vector3 v3 = new Vector3(x, y, z);
        NodeData n = Instantiate(node, v3, Quaternion.Euler(0f, 0f, 0f));
        n.Vector3 = v3;
        n.ID = id;
        n.Region = region;
        if (gender == "0")
        {
            n.Gender = "woman";
        } else if (gender == "1")
        {
            n.Gender = "man";
        }
        else
        {
            n.Gender = "N/A";
        }
        if (int.TryParse(age, out int value) && age != "0")
        {
            n.Age = age;
        }
        else
        {
            n.Age = "N/A";
        }
        n.name = n.ID.ToString();
        n.ForceVector3 = Vector3.zero;
        n.Show = false;
        n.gameObject.SetActive(false);
        return n;
    }

    public void FillEdges(List<int[]> lines, int from)
    {
        foreach (var line in lines)
        {
            NodeData n1 = nodes[((line[0]) - from)];
            NodeData n2 = nodes[((line[1]) - from)];
            int bil = line[2];
            EdgeData e = CreateEdge(n1, n2, bil);
            if (e != null)
            {
                edges.Add(e);
            }
        }
    }
    
    public EdgeData CreateEdge(NodeData n1, NodeData n2, int bil)
    {
        if (bil == 1)
        {
            EdgeData e = Instantiate(bilateral, n1.Vector3, Quaternion.Euler(0f, 0f, 0f));
            e.Bilateral = true;
            e.Node1 = n1;
            e.Node2 = n2;
            n1.getiNodes().Add(n2);
            n2.getiNodes().Add(n1);
            n1.getiEdges().Add(e);
            n2.getiEdges().Add(e);
            e.gameObject.SetActive(false);
            return e;
        }
        else
        {
            EdgeData e = Instantiate(edge, n1.Vector3, Quaternion.Euler(0f, 0f, 0f));
            e.Node1 = n1;
            e.Node2 = n2;
            n1.getiEdges().Add(e);
            n2.getiEdges().Add(e);
            n1.getiNodes().Add(n2);
            n2.getiNodes().Add(n1);
            e.gameObject.SetActive(false);
            return e;
        }
    }

    public void GraphSETActive(bool value)
    {
        foreach (var n in nodes)
        {
            n.Degree = n.getiNodes().Count; //resetDegree();
            n.ResetIDegree();
            n.ResetODegree();
        }
        if (activeEdges)
        {
            foreach (var e in edges)
            {
                e.gameObject.SetActive(value);
                e.Show = value;
            }
        }
        NodesActive(value);
        active = value;
    }

    public bool isGraphActive()
    {
        return active;
    }

    public void NodesActive(bool value)
    {
        foreach (var n in nodes)
        {
            n.gameObject.SetActive(value);
            n.Show = value;
        }
    }

    public void setEdgeVisibility(bool value)
    {
        foreach (var e in edges)
        {
            if (e.Node1.Show && e.Node2.Show)
            {
                e.gameObject.SetActive(value);
                e.Show = value;
            }
        }
    }

    public bool ActiveEdges
    {
        get => activeEdges;
        set => activeEdges = value;
    }

    private void RemoveAll()
    {
        foreach (var n in nodes)
        {
            n.gameObject.SetActive(false);
            var obj = n.gameObject;
            Destroy(obj);
        }
        nodes.Clear();
        foreach (var e in edges)
        {
            e.gameObject.SetActive(false);
            var obj = e.gameObject;
            Destroy(obj);
        }
        edges.Clear();
    }
}
