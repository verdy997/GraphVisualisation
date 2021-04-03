using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = System.Random;
using System;
using System.Globalization;

public class FileManager
{
    private List<string> lines{ get; set;}
    private string filePathNodes;
    private string filePathEdges;

    public FileManager(string fpNodes, string fpEdges)
    {
        filePathNodes = fpNodes;
        filePathEdges = fpEdges;
    }

    public bool readNodes(Graph graph)
    {
        Random rnd = new Random((int) DateTime.Now.Ticks & 0x0000FFFF);
        StreamReader sr = File.OpenText(filePathNodes);
        string s;
        graph.AddNode(null);
        while ((s = sr.ReadLine()) != null)
        {
            int value;
            string[] node = s.Split('\t');
            if (!int.TryParse(node[0], out value) || node.Length < 4)
            {
                Debug.Log(s);
                return false;
            }

            float x = rnd.Next(0, 10000)/100;
            float y = rnd.Next(0, 10000)/100;
            float z = rnd.Next(0, 10000)/100;
            Vector3 vector3 = new Vector3(x, y, z);
            graph.AddNode(new Node(int.Parse(node[0]), vector3, node[4]));
        }
        return true;
    }

    public bool readEdges(Graph graph)
    {
        StreamReader sr = File.OpenText(filePathEdges);
        string s;
        while ((s = sr.ReadLine()) != null)
        {
            string[] oNode = s.Split('\t');
            int value;
            if ((oNode.Length < 2) || !(int.TryParse(oNode[0], out value)) || !(int.TryParse(oNode[1], out value)))
            {
                Debug.Log(s);
                return false;
            }
            
            Node n1 = graph.getNodes()[int.Parse(oNode[0])];
            Node n2 = graph.getNodes()[int.Parse(oNode[1])];
            
            if (n1 != null && n2 != null)
            {
                if (n1.getiNodes().Contains(n2))
                {
                    for (int i = 0; i < n1.getiEdges().Count; i++)
                    {
                        if (n1.getiEdges()[i].Node1 == n2 || n1.getiEdges()[i].Node2 == n2)
                        {
                            n1.getiEdges()[i].setBilateral(true);
                            break;
                        }
                    }
                }
                else
                {
                    n1.AddiNode(n2);
                    n2.AddiNode(n1);
                    Edge e = new Edge(n1, n2, n1.Vector3);
                    graph.AddEdge(e);
                    n1.AddiEdge(e);
                    n2.AddiEdge(e);
                }
            }
            else
            {
                Debug.Log(s);
                return false;
            }
        }
        return true;
    }
}
