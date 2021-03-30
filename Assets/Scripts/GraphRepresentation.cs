using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Random = System.Random;

public class GraphRepresentation : MonoBehaviour
{
    public GameObject node;
    public GameObject edge;
    public GameObject emWrongFormat;
    public GameObject emNoPaths;
    public int from;
    public int to;
    public GameObject ifFrom;
    public GameObject ifTo;
    
    private FileManager fm;
    private List<GameObject> goEdges;
    private List<GameObject> goNodes;
    private Graph graph;
    private float maxDSquared = 10;
    private string pathToEdges;
    private string pathToNodes;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (graph != null)
        {
            if ((goNodes.Count != 0) && (goEdges.Count != 0))
            {
                
                ForceDirect();

                //update of nodes (gameobjects)
                foreach (var node in goNodes)
                {
                    if (node != null)
                    {
                        node.transform.SetPositionAndRotation(graph.getNodes()[int.Parse(node.name)].Vector3,
                            Quaternion.Euler(0f, 0f, 0f));
                    }
                }

                //update of edges (gameobjects)
                for (int j = 0; j < goEdges.Count; j++)
                {
                    float distance = Vector3.Distance(graph.getEdges()[j].Node1.Vector3,
                        graph.getEdges()[j].Node2.Vector3);
                    Vector3 newZ = goEdges[j].transform.localScale;
                    newZ.z = distance / 2;
                    goEdges[j].transform.localScale = newZ;
                    goEdges[j].transform.position = graph.getEdges()[j].Node1.Vector3;
                    goEdges[j].transform.LookAt(graph.getEdges()[j].Node2.Vector3);
                }
            }
        }
    }

    public void FindNodes()
    {
        pathToNodes = EditorUtility.OpenFilePanel("Searching for nodes", "", "txt");
    }

    public void FindEdges()
    {
        pathToEdges = EditorUtility.OpenFilePanel("Searching for edges", "", "txt");
    }

    public void setRange()
    {
        int value;
        if (int.TryParse(ifFrom.GetComponent<Text>().text, out value) && 
            int.TryParse(ifTo.GetComponent<Text>().text, out value))
        {
            int fromInt = int.Parse(ifFrom.GetComponent<Text>().text);
            int toInt = int.Parse(ifTo.GetComponent<Text>().text);
        }
    }


    public void CreateGraph()
    {
        graph = new Graph();
        goEdges = new List<GameObject>();
        goNodes = new List<GameObject>();
        if (pathToEdges == null || pathToNodes == null)
        {
            GameObject canvas = GameObject.Find("Canvas");
            GameObject error = Instantiate(emNoPaths);
            error.transform.SetParent(canvas.transform, false);
            return;
        }

        fm = new FileManager(pathToNodes, pathToEdges);
        if (!fm.readNodes(graph) || !fm.readEdges(graph))
        {
            GameObject canvas = GameObject.Find("Canvas");
            GameObject error = Instantiate(emWrongFormat);
            error.transform.SetParent(canvas.transform, false);
            return;
        }

        //List<Node> nodes = graph.getNodes();

        foreach (Node nodeInc in graph.getNodes())
        {
            if (nodeInc != null)
            {
                GameObject newNode = Instantiate(node, nodeInc.Vector3, Quaternion.Euler(0f, 0f, 0f));
                newNode.name = nodeInc.ID.ToString();
                goNodes.Add(newNode);
            }
            else
            {
                goNodes.Add(null);
            }
        }

        //List<Edge> edges = graph.getEdges();
        foreach (Edge edgeInc in graph.getEdges())
        {
            GameObject newEdge = Instantiate(edge, edgeInc.Vector3, Quaternion.Euler(0f, 0f, 0f));
            newEdge.name = "edge" + edgeInc.Node1.ID.ToString() + "-" + edgeInc.Node2.ID.ToString();
            newEdge.transform.LookAt(edgeInc.Node2.Vector3);
            float distance = Vector3.Distance(edgeInc.Node1.Vector3, edgeInc.Node2.Vector3);
            Vector3 newZ = newEdge.transform.localScale;
            newZ.z = distance;
            newEdge.transform.localScale = newZ / 2;
            goEdges.Add(newEdge);
        }
    }

    void ForceDirect()
    {
        int _L = 150; //spring rest length
        int _K_r = 200; //repulsive force constant
        float _K_s = 1; //spring constant
        float delta_t = 0.0005f; //time step
        int _N = graph.getNodes().Count;

        //inicializacia net force
        foreach (var node in graph.getNodes())
        {
            if (node != null)
            {
                node.setForceVector3(0, 0, 0);
            }
        }

        //odpor medzi dvojicami
        for (int i1 = 0; i1 <= _N - 2; i1++)
        {
            bool expression1 = true;
            while (expression1)
            {
                if (graph.getNodes()[i1] != null)
                {
                    expression1 = false;
                }
                else
                {
                    i1++;
                }
            }

            Node node1 = graph.getNodes()[i1];

            for (int i2 = i1 + 1; i2 <= _N - 1; i2++)
            {
                bool expression2 = true;
                while (expression2)
                {
                    if (graph.getNodes()[i2] != null)
                    {
                        expression2 = false;
                    }
                    else
                    {
                        i2++;
                    }
                }

                Node node2 = graph.getNodes()[i2];
                Vector3 dv3 = node2.Vector3 - node1.Vector3;

                if (dv3.x != 0 && dv3.y != 0 && dv3.z != 0)
                {
                    float distanceSquared = dv3.x * dv3.x + dv3.y * dv3.y + dv3.z * dv3.z;
                    float distance = Mathf.Sqrt(distanceSquared);
                    float force = _K_r / distanceSquared;
                    Vector3 fv3 = force * dv3 / distance;
                    node1.setForceVector3(node1.ForceVector3.x - fv3.x,
                        node1.ForceVector3.y - fv3.y,
                        node1.ForceVector3.z - fv3.z);

                    node2.setForceVector3(node2.ForceVector3.x + fv3.x,
                        node2.ForceVector3.y + fv3.y,
                        node2.ForceVector3.z + fv3.z);
                }
            }
        }

        //spring force between adjacent pairs
        for (int i1 = 0; i1 <= _N - 1; i1++)
        {
            bool expression1 = true;
            while (expression1)
            {
                if (graph.getNodes()[i1] != null)
                {
                    expression1 = false;
                }
                else
                {
                    i1++;
                }
            }

            Node node1 = graph.getNodes()[i1];

            for (int j = 0; j <= node1.getiNodes().Count - 1; j++)
            {
                int i2 = node1.getiNodes()[j].ID;
                Node node2 = graph.getNodes()[i2];
                if (i1 < i2)
                {
                    Vector3 dv3 = node2.Vector3 - node1.Vector3;
                    if (dv3.x != 0 || dv3.y != 0 || dv3.z != 0)
                    {
                        float distanceSquared = dv3.x * dv3.x + dv3.y * dv3.y + dv3.z * dv3.z;
                        float distance = Mathf.Sqrt(distanceSquared);
                        float force = _K_s * (distance - _L);
                        Vector3 fv3 = force * dv3 / distance;

                        node1.setForceVector3(node1.ForceVector3.x - fv3.x,
                            node1.ForceVector3.y - fv3.y,
                            node1.ForceVector3.z - fv3.z);

                        node2.setForceVector3(node2.ForceVector3.x + fv3.x,
                            node2.ForceVector3.y + fv3.y,
                            node2.ForceVector3.z + fv3.z);
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
            bool expression = true;
            while (expression)
            {
                if (graph.getNodes()[i] != null)
                {
                    expression = false;
                }
                else
                {
                    i++;
                }
            }

            Node node = graph.getNodes()[i];
            Vector3 dv3 = delta_t * node.ForceVector3;
            float displaceSquered = dv3.x * dv3.x + dv3.y * dv3.y + dv3.z * dv3.z;

            if (displaceSquered > maxDSquared)
            {
                float s = Mathf.Sqrt(maxDSquared / displaceSquered);
                dv3.x *= s;
                dv3.y *= s;
                dv3.z *= s;
                //maxDSquared = displaceSquered;
            }

            node.setVector3((node.Vector3.x + dv3.x), (node.Vector3.y + dv3.y), (node.Vector3.z + dv3.z));
        }
    }
}