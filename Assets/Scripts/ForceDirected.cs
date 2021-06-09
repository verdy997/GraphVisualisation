using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Random = System.Random;

public class ForceDirected : MonoBehaviour
{
    public void ParallelFD(int L, int Kr, int Ks, float deltaT, int N, List<NodeData> nodes)
    {
        SetZeroVector(nodes);
        
        Parallel.For(0, N - 2, i1 =>
        {
            NodeData node1 = nodes[i1];
            for (int i2 = i1 + 1; i2 <= N - 1; i2++)
            {
                NodeData node2 = nodes[i2];
                RepulsionBtwnAllPairs(node1, node2, Kr, 1f);
            }
        });
        
        Parallel.For(0, N - 1, i1 =>
        {
            SpringBtwnAdjPairs(nodes, i1, L, Ks);
        });
        
        UpdatePosition(N, deltaT, nodes);
    }
    
    public void SequentialFD(int L, int Kr, int Ks, float deltaT, int N, List<NodeData> nodes)
    {
        SetZeroVector(nodes);

        for (int i1 = 0; i1 <= N - 2; i1++)
        {
            NodeData node1 = nodes[i1];
            //odpor medzi dvojicami
            for (int i2 = i1 + 1; i2 <= N - 1; i2++)
            {
                NodeData node2 = nodes[i2];
                float bulk = Mathf.Max(node1.transform.localScale.x, node2.transform.localScale.x);
                RepulsionBtwnAllPairs(node1, node2, Kr, bulk);
                
            }
        }

        //spring force between adjacent pairs
        for (int i1 = 0; i1 <= N - 1; i1++)
        {
            SpringBtwnAdjPairs(nodes, i1, L, Ks);
        }
        
        UpdatePosition(N, deltaT, nodes);
        
    }

    private void SetZeroVector(List<NodeData> nodes)
    {
        foreach (var node in nodes)
        {
            node.ForceVector3 = Vector3.zero;
        }
    }

    private void RepulsionBtwnAllPairs(NodeData node1, NodeData node2, int Kr, float bulk)
    {
        Vector3 dv3 = node2.Vector3 - node1.Vector3;
        if (dv3 != Vector3.zero)
        {
            int max = Mathf.Min(node1.getiNodes().Count, node2.getiNodes().Count);
            if (Mathf.Min(node1.getiNodes().Count, node2.getiNodes().Count) > 50)
            {
                max = 50;
            }
                
            float distanceSquared = dv3.x * dv3.x + dv3.y * dv3.y + dv3.z * dv3.z;
            float distance = Mathf.Sqrt(distanceSquared);
            float force = (Kr * (bulk * 3.14f)  * max) / (distanceSquared);
            Vector3 fv3 = (force * dv3) / distance;
            node1.ForceVector3 = node1.ForceVector3 - fv3;
            node2.ForceVector3 = node2.ForceVector3 + fv3;
        }
    }

    private void SpringBtwnAdjPairs(List<NodeData> nodes, int i, int L, int Ks)
    {
        NodeData node1 = nodes[i];

        for (int j = 0; j <= node1.getiNodes().Count - 1; j++)
        {
            NodeData node2 = node1.getiNodes()[j];
            if (node1.ID < node2.ID)
            {
                Vector3 dv3 = node2.Vector3 - node1.Vector3;
                if (dv3 != Vector3.zero)
                {
                    float distanceSquared = dv3.x * dv3.x + dv3.y * dv3.y + dv3.z * dv3.z;
                    float distance = Mathf.Sqrt(distanceSquared);
                    float force = Ks * (distance - L);
                    Vector3 fv3 = force * dv3 / (distance);
                    node1.ForceVector3 = node1.ForceVector3 - fv3;
                    node2.ForceVector3 = node2.ForceVector3 + fv3;
                }
                else
                {
                    Random rnd = new Random(Guid.NewGuid().GetHashCode());
                    float x = (float) rnd.Next(0, 10) / 100000f;
                    float y = (float) rnd.Next(0, 10) / 100000f;
                    float z = (float) rnd.Next(0, 10) / 100000f;
                    node1.SetForceVector3(node1.ForceVector3.x - x,
                        node1.ForceVector3.y - y,
                        node1.ForceVector3.z - z);

                    node2.SetForceVector3(node2.ForceVector3.x + x,
                        node2.ForceVector3.y + y,
                        node2.ForceVector3.z + z);
                }
            }
        }
    }

    private void UpdatePosition(int N, float deltaT, List<NodeData> nodes)
    {
        int maxDSquared = 50;
        for (int i = 0; i <= N - 1; i++)
        {
            NodeData node = nodes[i];
            Vector3 dv3 = deltaT * nodes[i].ForceVector3;
            float displaceSquered = dv3.x * dv3.x + dv3.y * dv3.y + dv3.z * dv3.z;
            if (displaceSquered > maxDSquared)
            {
                float s = Mathf.Sqrt(maxDSquared / displaceSquered);
                dv3.x *= s;
                dv3.y *= s;
                dv3.z *= s;
            }
            nodes[i].Vector3 = nodes[i].Vector3 + dv3;
        }
    }
}
