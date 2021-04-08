using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceDirected : MonoBehaviour
{
    public void ForceDirect(int _L, int _K_r, int _K_s, float delta_t, int _N, int maxDSquared, GraphData graph)
    {
        /*int _L = 200; //spring rest length 200
        int _K_r = 750 + (graph.getNodes().Count/1000); //repulsive force constant 200
        int _K_s = 1; //spring constant 1
        float delta_t = 0.05f; //time step 0.005
        int _N = graph.getNodes().Count;
        int maxDSquared = 50;*/

        foreach (var node in graph.getNodes())
        {
            node.ForceVector3 = Vector3.zero;
        }
        
        //odpor medzi dvojicami
        //int pom = 1;
        //NodeData n1 = graph.getNodes().Where(obj => obj.ID == pom).First();
        for (int i1 = 0; i1 <= _N - 2; i1++)
        {
            NodeData node1 = graph.getNodes()[i1];
            for (int i2 = i1 + 1; i2 <= _N - 1; i2++)
            {
                NodeData node2 = graph.getNodes()[i2];
                Vector3 dv3 = node2.Vector3 - node1.Vector3;
                if (dv3 != Vector3.zero)
                {
                    int max;
                    if (Mathf.Min(node1.getiNodes().Count, node2.getiNodes().Count) > 50)
                    {
                        max = 50;
                    }
                    else
                    {
                        max = Mathf.Min(node1.getiNodes().Count, node2.getiNodes().Count);
                    }

                    float bulk = Mathf.Max(node1.transform.localScale.x, node2.transform.localScale.x);
                    float distanceSquared = dv3.x * dv3.x + dv3.y * dv3.y + dv3.z * dv3.z;
                    float distance = Mathf.Sqrt(distanceSquared);
                    float force = (_K_r * (bulk)  * max) / (distanceSquared);
                    Vector3 fv3 = (force * dv3) / distance;
                    node1.ForceVector3 = node1.ForceVector3 - fv3;
                    node2.ForceVector3 = node2.ForceVector3 + fv3;
                }
            }
        }

        //spring force between adjacent pairs
        for (int i1 = 0; i1 <= _N - 1; i1++)
        {
            NodeData node1 = graph.getNodes()[i1];

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
                        float force = _K_s * (distance - _L );
                        Vector3 fv3 = force * dv3 / distance;
                        node1.ForceVector3 = node1.ForceVector3 - fv3;
                        node2.ForceVector3 = node2.ForceVector3 + fv3;
                    }
                    else
                    {
                        node1.setForceVector3(node1.ForceVector3.x - 0.001f,
                            node1.ForceVector3.y - 0.001f,
                            node1.ForceVector3.z - 0.001f);

                        node2.setForceVector3(node2.ForceVector3.x + 0.001f,
                            node2.ForceVector3.y + 0.001f,
                            node2.ForceVector3.z + 0.001f);
                    }
                }
            }
        }

        //update positions
        for (int i = 0; i <= _N - 1; i++)
        {
            NodeData node = graph.getNodes()[i];
            Vector3 dv3 = delta_t * graph.getNodes()[i].ForceVector3;
            float displaceSquered = dv3.x * dv3.x + dv3.y * dv3.y + dv3.z * dv3.z;

            if (displaceSquered > maxDSquared)
            {
                float s = Mathf.Sqrt(maxDSquared / displaceSquered);
                dv3.x *= s;
                dv3.y *= s;
                dv3.z *= s;
            }
            graph.getNodes()[i].Vector3 = graph.getNodes()[i].Vector3 + dv3;
        }
    }
}
