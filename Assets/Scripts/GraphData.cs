using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class GraphData : MonoBehaviour
{
    public NodeData node;
    public EdgeData edge;
    public EdgeData bilateral;
    
    private List<NodeData> nodes = new List<NodeData>();
    private List<EdgeData> edges = new List<EdgeData>();
    private bool active = false;
    
    private void Start()
    {
        name = "Graph";
    }

    public void AddNode(NodeData node)
    {
        nodes.Add(node);
    }

    public void AddEdge(EdgeData edge)
    {
        edges.Add(edge);
    }

    public List<NodeData> getNodes()
    {
        return nodes;
    }

    public List<EdgeData> getEdges()
    {
        return edges;
    }

    public void setRange(int fromN, int toN, List<string[]> linesN, List<string[]> linesE)
    {
        FillNodes((linesN.Where(nline => int.Parse(nline[0]) >= fromN && int.Parse(nline[0]) <= toN)).ToList());
        
        FillEdges((linesE.Where(eline => int.Parse(eline[0]) >= fromN && int.Parse(eline[0]) <= toN &&
                                             int.Parse(eline[1]) >= fromN && int.Parse(eline[1]) <= toN)).ToList());
    }

    /*public void createRegionGraph(List<string[]> linesN, List<string[]> linesE)
    {
        List<string> regions = new List<string>();
        
        foreach (var line in linesN)
        {
            if (!regions.Contains(line[4]))
            {
                regions.Add(line[4]);
                Subgraph sub = Instantiate(subgraph);
                sub.ID = subgraphs.Count;
                sub.Region = line[4];
                sub.getNodes().Add(CreateNode(int.Parse(line[0]), line[4], 0));
                subgraphs.Add(sub);
            }
            else
            {
                regions.IndexOf(line[4]);
                subgraphs[regions.IndexOf(line[4])].getNodes().Add(CreateNode(int.Parse(line[0]), line[4], 0));
            }
        }

        foreach (var sub in subgraphs)
        {
            NodeData n = CreateNode(sub.ID, sub.Region, sub.getNodes().Count);
            n.gameObject.SetActive(true);
            Vector3 bulk = n.transform.localScale;
            bulk.x = (float) (bulk.x + ((double)sub.getNodes().Count / 100));
            bulk.y = (float) (bulk.y + ((double)sub.getNodes().Count / 100));
            bulk.z = (float) (bulk.z + ((double)sub.getNodes().Count / 100));
            n.transform.localScale = bulk;
            n.tag = "Cluster";
            n.Cluster = true;
            AddNode(n);
        }
        
        //hrany -> do podgrafov (iba tie ktore obsahuju node)
        foreach (var line in linesE)
        {
            int n1 = int.Parse(line[0]);
            int n2 = int.Parse(line[1]);
            IEnumerable<Subgraph> s = subgraphs.Where(sub => sub.ContainsId(n1) && sub.ContainsId(n2));
            if (s.Count() > 0)
            {
                Subgraph sg = s.Single();
                NodeData node1 = sg.getNodes().Where(n => n.ID == n1).Single();
                NodeData node2 = sg.getNodes().Where(n => n.ID == n2).Single();
                EdgeData e = CreateEdge(node1, node2, 0, sg.getEdges());
                if (e != null)
                {
                    sg.getEdges().Add(e);
                }
            }
            else
            {
                Subgraph s1 = subgraphs.Where(sub => sub.ContainsId(n1)).Single();
                Subgraph s2 = subgraphs.Where(sub => sub.ContainsId(n2)).Single();
                NodeData node1 = nodes.Where(node => node.ID == s1.ID).Single();
                NodeData node2 = nodes.Where(node => node.ID == s2.ID).Single();
                EdgeData e = CreateEdge(node1, node2, 1, edges);
                if (e != null)
                {
                    edges.Add(e);
                }
            }
        }
    }*/

    public void FillNodes(List<string[]> lines)
    {
        foreach (var line in lines)
        {
            NodeData n = CreateNode(int.Parse(line[0]), line[4]);
            AddNode(n);
        }
    }

    public NodeData CreateNode(int id, string region)
    {
        Random rnd = new Random((int) DateTime.Now.Ticks & 0x0000FFFF);
        float x = rnd.Next(0, 10000) / 100f;
        float y = rnd.Next(0, 10000) / 100f;
        float z = rnd.Next(0, 10000) / 100f;
        Vector3 v3 = new Vector3(x, y, z);
        NodeData n = Instantiate(node, v3, Quaternion.Euler(0f, 0f, 0f));
        n.Vector3 = v3;
        n.ID = id;
        n.Region = region;
        n.ForceVector3 = Vector3.zero;
        n.gameObject.SetActive(false);
        return n;
    }

    public void FillEdges(List<string[]> lines)
    {
        foreach (var line in lines)
        {
            NodeData n1 = nodes.Where(n => n.ID == int.Parse(line[0])).Single();
            NodeData n2 = nodes.Where(n => n.ID == int.Parse(line[1])).Single();
            n1.Degree++;
            n2.Degree++;
            EdgeData e = CreateEdge(n1, n2);
            if (e != null)
            {
                AddEdge(e);
            }
        }
    }

    public EdgeData CreateEdge(NodeData n1, NodeData n2)
    {
        if (n1.getiNodes().Contains(n2))
        {
            IEnumerable<EdgeData> en = edges.Where(ed => ed.Node1 == n1 && ed.Node2 == n2 || ed.Node1 == n2 && ed.Node2 == n1);
            if (en.Count() > 0)
            {
                en.Single().Bilateral = true;
            }
            return null;
        }
        else
        {
            n1.AddiNode(n2);
            n2.AddiNode(n1);
            EdgeData e = Instantiate(edge, n1.Vector3, Quaternion.Euler(0f, 0f, 0f));
            e.Node1 = n1;
            e.Node2 = n2;
            n1.AddiEdge(e);
            n2.AddiEdge(e);
            e.gameObject.SetActive(false);
            return e;
        }
    }

    public void GraphSETActive(bool value)
    {
        NodeActive(value);
        EdgeActive(value);
        active = value;
    }

    public bool isGraphActive()
    {
        return active;
    }

    public void NodeActive(bool value)
    {
        foreach (var n in nodes)
        {
            n.gameObject.SetActive(value);
        }
    }

    public void EdgeActive(bool value)
    {
        foreach (var e in edges)
        {
            e.gameObject.SetActive(value);
        }
    }

}
