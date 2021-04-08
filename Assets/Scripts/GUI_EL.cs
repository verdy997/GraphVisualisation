using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class GUI_EL : MonoBehaviour
{
    //public GraphRepresentation gr;
    public GraphRepresentation2 gr;
    public GameObject nodeInfo;
    public Text iDNode;
    public Text iNodes;
    public GameObject emNoPaths;
    public GameObject emWrongFormat;
    public GameObject pleaseWait;
    public FileManager2 fm;
    public Text fromN;
    public Text toN;
    public GraphData graph;

    private string pathToEdges;
    private string pathToNodes;
    
    public void ShowInfo(GameObject go)
    {
        if (go.tag == "Node")
        {
            int id = int.Parse(go.name);
            var node = gr.getGraph().getNodes()[id-1];
            string s = "";
            for (int i = 0; i < node.getiNodes().Count; i++)
            {
                s += node.getiNodes()[i].ID + ", ";
            }

            iDNode.GetComponent<Text>().text = id.ToString() +" " + node.Region;
            iNodes.GetComponent<Text>().text = s;
            nodeInfo.SetActive(true);
        }
        else if (go.tag == "Cluster")
        {
            int id = int.Parse(go.name);
            var node = gr.getGraph().getNodes()[id];
            string s = "";
            foreach (var inode in node.getiNodes())
            {
                s += inode.Region + " ";
            }
            
            iDNode.GetComponent<Text>().text = id.ToString() +" " + node.Region;
            iNodes.GetComponent<Text>().text = s;
            nodeInfo.SetActive(true);
        }
    }

    public void showPleaseWait(bool value)
    {
        pleaseWait.gameObject.SetActive(value);
    }
    
    public void FindNodes()
    {
        pathToNodes = EditorUtility.OpenFilePanel("Searching for nodes", "", "txt");
    }

    public void FindEdges()
    {
        pathToEdges = EditorUtility.OpenFilePanel("Searching for edges", "", "txt");
    }

    public void setEdgeVisibility(bool value)
    {
        graph.EdgeActive(value);
    }
    
    public void LoadData()
    {
        if (pathToEdges == null || pathToNodes == null)
        {
            emNoPaths.SetActive(true);
            return;
        }

        if (!fm.ReadNodes(pathToNodes) || !fm.ReadEdges(pathToEdges))
        {
            emWrongFormat.SetActive(true);
            return;
        }

        if (fromN.text.Length == 0)
        {
            if (toN.text.Length == 0)
            {
                fm.SendData();
            }
            else
            {
                fm.SendData(0,int.Parse(toN.text));
            }
        }
        else
        {
            if (toN.text.Length == 0)
            {
                fm.SendData(int.Parse(fromN.text), 0);
            }
            else
            {
                fm.SendData(int.Parse(fromN.text), int.Parse(toN.text));
            }
        }
    }
}
