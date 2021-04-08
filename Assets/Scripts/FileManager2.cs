using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = System.Random;
using System;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;

public class FileManager2 : MonoBehaviour
{
    public GraphData graph;
    private List<string[]> nodelines = new List<string[]>();
    private List<string[]> edgelines = new List<string[]>();
    
    public bool ReadNodes(string filePathNodes)
    {
        if (nodelines.Count > 0)
        {
            int tmp = nodelines.Count;
            nodelines.RemoveRange(0, tmp);
        }
        StreamReader sr = File.OpenText(filePathNodes);
        string s;
        while ((s = sr.ReadLine()) != null)
        {
            int value;
            string[] node = s.Split('\t');
            if (!int.TryParse(node[0], out value) || node.Length < 4)
            {
                return false;
            }
            nodelines.Add(node);
        }
        return true;
    }

    public bool ReadEdges(string filePathEdges)
    {
        if (edgelines.Count > 0)
        {
            int tmp = edgelines.Count;
            edgelines.RemoveRange(0, tmp);
        }
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
            edgelines.Add(oNode);
        }
        return true;
    }

    public void SendData()
    {
        graph.FillNodes(nodelines);
        graph.FillEdges(edgelines);
    }

    public void SendData(int from, int to)
    {
        graph.setRange(from, to , nodelines, edgelines);
    }
}
