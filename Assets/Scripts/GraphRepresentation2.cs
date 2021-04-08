using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

public class GraphRepresentation2 : MonoBehaviour
{
    public GraphData graph;
    public ForceDirected fd;
    public GUI_EL myGui;

    public GraphData getGraph()
    {
        return graph;
    }

    public void CalculatePosition()
    {
        myGui.showPleaseWait(true);
        for (int i = 0; i < 40; i++)
        {
            fd.ForceDirect(200, 750, 1, 0.05f, graph.getNodes().Count, 50, graph);
        }

        if (graph.isGraphActive())
        {
            ShowGraph();
        }
        myGui.showPleaseWait(false);
    }

    public void ShowGraph()
    {
        graph.GraphSETActive(true);
        foreach (var node in graph.getNodes())
        {
            node.transform.SetPositionAndRotation(node.Vector3, Quaternion.Euler(0f, 0f, 0f));
        }
        foreach (var edge in graph.getEdges())
        {
            edge.UpdatePosition();
        }
    }
    
    
}
