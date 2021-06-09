using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = System.Random;
using System;
//using System.Diagnostics;
//using System.Globalization;
//using System.Runtime.Serialization.Formatters.Binary;
using Debug = UnityEngine.Debug;

public class FileManager : MonoBehaviour
{
    [SerializeField] private GraphData graph;
    private List<string[]> nodelines = new List<string[]>();
    //private List<string[]> edgelines = new List<string[]>();
    private List<int[]> edges = new List<int[]>();
    
    
    public bool ReadNodes(string filePathNodes, int to)
    {
        if (nodelines.Count > 0)
        {
            nodelines.Clear();
        }
        StreamReader sr = File.OpenText(filePathNodes);
        string s;
        while ((s = sr.ReadLine()) != null)
        {
            int value;
            string[] node = s.Split('\t');
            if (!int.TryParse(node[0], out value) || node.Length < 7)
            {
                sr.Close();
                return false;
            }

            if (to != 0)
            {
                if (int.Parse(node[0]) > to )
                {
                    sr.Close();
                    return true;
                }
            }
            nodelines.Add(node);
        }
        sr.Close();
        return true;
    }

    public bool ReadEdges(string filePathEdges, int to)
    {
        if (edges.Count > 0)
        {
            edges.Clear();
        }
        StreamReader sr = File.OpenText(filePathEdges);
        string s;
        while ((s = sr.ReadLine()) != null)
        {
            string[] oNode = s.Split('\t');
            if ((oNode.Length < 3) || !(int.TryParse(oNode[0], out int k)) || !(int.TryParse(oNode[1], out int j)) 
                || !(int.TryParse(oNode[2], out int g)))
            {
                sr.Close();
                return false;
            }
            if (to != 0)
            {
                if (int.Parse(oNode[0]) > to)
                {
                    sr.Close();
                    return true;
                }
            }
            edges.Add(new[] {int.Parse(oNode[0]), int.Parse(oNode[1]), int.Parse(oNode[2])});
        }
        sr.Close();
        return true;
    }

    public void SendData()
    {
        graph.FillNodes(nodelines);
        graph.FillEdges(edges, 1);
    }

    public void SendData(int from, int to)
    {
        graph.setRange(from, to , nodelines, edges);
    }
    

    public void SaveData(string nameFolder)
    {
        System.IO.Directory.CreateDirectory("Projects\\" +nameFolder);
        string pathN = "Projects\\" + nameFolder + "\\nodes.txt";
        string pathE = "Projects\\" + nameFolder + "\\edges.txt";

        using (StreamWriter sw = new StreamWriter(pathN))
        {
            foreach (var n in graph.getNodes())
            {
                sw.WriteLine(n.ID.ToString() + '\t' + ((double)n.Vector3.x).ToString() + '\t' + ((double)n.Vector3.y).ToString()
                             + '\t' + n.Gender + '\t' + n.Region + '\t' + ((double)n.Vector3.z).ToString() + '\t' + " " + '\t' + n.Age);
            }
        }

        using (StreamWriter sw = new StreamWriter(pathE))
        {
            foreach (var e in graph.getEdges())
            {
                int bil = 0;
                if (e.Bilateral)
                {
                    bil = 1;
                }
                sw.WriteLine(e.Node1.ID.ToString() + '\t' + e.Node2.ID.ToString() + '\t' + bil.ToString());
            }
        }
    }

}
