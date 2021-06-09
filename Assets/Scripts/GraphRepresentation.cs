using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class GraphRepresentation : MonoBehaviour
{
    [SerializeField] private GraphData graph;
    [SerializeField] private ForceDirected fd;
    [SerializeField] private RegionGraph rg;

    public GraphData GetGraph()
    {
        return graph;
    }

    public void CalculatePositionParallel(int spring, int kr, int nOrep, float ts)
    {
        for (int i = 0; i < nOrep; i++)
        {
            fd.ParallelFD(spring, kr, 1, ts, graph.getNodes().Count, graph.getNodes());
        }
        if (graph.isGraphActive())
        {
            ShowGraph();
        }
    }
    
    public void CalculatePositionSequential(int spring, int kr, int nOrep, float ts)
    {
        for (int i = 0; i < nOrep; i++)
        {
            fd.SequentialFD(spring, kr, 1, ts, graph.getNodes().Count, graph.getNodes());
        }

        if (graph.isGraphActive())
        {
            ShowGraph();
        }
    }

    public void ShowGraph()
    {
        if (rg.Active)
        {
            rg.DeactivateRegionGraph();
        }
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

    public void CreateRegionGraph()
    {
        if (rg.getRegionNodes().Count == 0)
        {
            rg.CreateRegionGraph(graph.getNodes(), graph.getEdges());
            rg.CalculatePositionClusters();
        }
        rg.ShowRegionGraph();
    }
}
