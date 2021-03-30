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
        while ((s = sr.ReadLine()) != null)
        {
            int value;
            string[] node = s.Split('\t');
            if (!int.TryParse(node[0], out value))
            {
                return false;
            }

            float x = rnd.Next(0, 1000)/100;
            float y = rnd.Next(0, 1000)/100;
            float z = rnd.Next(0, 1000)/100;
            Vector3 vector3 = new Vector3(x, y, z);
            if (int.Parse(node[0]) == graph.getNodes().Count)
            {
                graph.AddNode(new Node(int.Parse(node[0]), vector3));
            }
            else
            {
                for (int i = graph.getNodes().Count; i <= int.Parse(node[0]); i++)
                {
                    graph.AddNode(null);
                }
                graph.AddNode(int.Parse(node[0]), new Node(int.Parse(node[0]), vector3));
            }
        }
        /*List<string> lines = File.ReadLines(filePathNodes).ToList();
        foreach (var line in lines)
        {
            int value;
            string[] node = line.Split('\t');
            if (!int.TryParse(node[0], out value))
            {
                return false;
            }
            if (int.Parse(node[0]) == 1632803)
            {
                Debug.Log("end");
                Debug.Log("im here mtfck");
                return false;
            }
            
            float x = rnd.Next(0, 1000)/100;
            float y = rnd.Next(0, 1000)/100;
            float z = rnd.Next(0, 1000)/100;
            Vector3 vector3 = new Vector3(x, y, z);
            if (int.Parse(node[0]) == graph.getNodes().Count)
            {
                graph.AddNode(new Node(int.Parse(node[0]), vector3));
            }
            else
            {
                for (int i = graph.getNodes().Count; i <= int.Parse(node[0]); i++)
                {
                    graph.AddNode(null);
                }
                graph.AddNode(int.Parse(node[0]), new Node(int.Parse(node[0]), vector3));
            }
        }*/
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
                return false;
            }
            Node n1 = graph.getNodes()[int.Parse(oNode[0])];
            Node n2 = graph.getNodes()[int.Parse(oNode[1])];
            
            if (n1 != null && n2 != null)
            {
                //Node n1 = graph.getNodeByID(int.Parse(oNode[0]));
                //Node n1 = graph.getNodes()[int.Parse(oNode[0])];
                //Node n2 = graph.getNodes()[int.Parse(oNode[1])];
                n1.AddiNode(n2);
                n2.AddiNode(n1);
                //Vector3 vector3 = Vector3.Lerp(n1.Vector3, n2.Vector3, 0.5f);
                graph.AddEdge(new Edge(n1, n2, n1.Vector3));
            }
            else
            {
                return false;
            }
            
        }

        /*List<string> lines = File.ReadLines(filePathEdges).ToList();
        foreach (var line in lines)
        {
            string[] oNode = line.Split('\t');
            int value;
            if ((oNode.Length < 2) || !(int.TryParse(oNode[0], out value)) || !(int.TryParse(oNode[1], out value)))
            {
                return false;
            }
            if (graph.getNodes()[int.Parse(oNode[0])] != null && graph.getNodes()[int.Parse(oNode[1])] != null)
            {
                //Node n1 = graph.getNodeByID(int.Parse(oNode[0]));
                Node n1 = graph.getNodes()[int.Parse(oNode[0])];
                Node n2 = graph.getNodes()[int.Parse(oNode[1])];
                n1.AddiNode(n2);
                n2.AddiNode(n1);
                //Vector3 vector3 = Vector3.Lerp(n1.Vector3, n2.Vector3, 0.5f);
                graph.AddEdge(new Edge(n1, n2, n1.Vector3));
                
                if (int.Parse(oNode[0]) == 1632803)
                {
                    Debug.Log("im here mtfck");
                }
            }
            else
            {
                return false;
            }
        }*/
        return true;
    }
}
