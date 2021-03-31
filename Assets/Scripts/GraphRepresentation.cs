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
    public GameObject emNoNode;
    public int from;
    public int to;
    public GameObject ifFrom;
    public GameObject ifTo;
    public GameObject ifFirstN;
    
    
    private FileManager fm;
    private List<GameObject> goEdges;
    private List<GameObject> goNodes;
    private Graph graph;
    private float maxDSquared = 10;
    private string pathToEdges;
    private string pathToNodes;
    private string neighbor;
    private Ray ray;
    private RaycastHit hit;

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

    public Graph getGraph()
    {
        return graph;
    }

    public List<GameObject> getEdges()
    {
        return goEdges;
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

    private void AddNewNode(int id)
    {
        Random rnd = new Random((int) DateTime.Now.Ticks & 0x0000FFFF);
        float x = rnd.Next(0, 1000)/100;
        float y = rnd.Next(0, 1000)/100;
        float z = rnd.Next(0, 1000)/100;
        Vector3 v3 = new Vector3(x, y, z);
        Node n1 = new Node(id, v3);
        if (id == graph.getNodes().Count)
        {
            graph.AddNode(n1);
        }
        else
        {
            graph.AddNode(id, n1);
        }

    }

    private Edge CreateEdge(Node n1, Node n2)
    {
        n1.AddiNode(n2);
        n2.AddiNode(n1);
        Edge e1 = new Edge(n1, n2, n1.Vector3);
        return e1;
    }
    
    private void AddNewEdge(Edge e)
    {
        graph.AddEdge(e);
    }

    private void DrawNode(Node n)
    {
        GameObject newNode = Instantiate(node, n.Vector3, Quaternion.Euler(0f, 0f, 0f));
        newNode.name = n.ID.ToString();
        goNodes.Add(newNode);
    }

    private void DrawEdge(Edge e)
    {
        GameObject newEdge = Instantiate(edge, e.Vector3, Quaternion.Euler(0f, 0f, 0f));
        newEdge.name = "." + e.Node1.ID.ToString() + ".-." + e.Node2.ID.ToString() + ".";
        goEdges.Add(newEdge);
    }

    public void AddNewNodeToGraph()
    {
        int n1ID;
        int n2ID;
        int value;
        if (graph != null)
        {
            n1ID = graph.getNodes().Count;
            AddNewNode(n1ID);
            DrawNode(graph.getNodes()[n1ID]);
            if (int.TryParse(ifFirstN.GetComponent<Text>().text, out value))
            {
                n2ID = int.Parse(ifFirstN.GetComponent<Text>().text);
                if (graph.getNodes().Count < n2ID || graph.getNodes()[n2ID] == null)
                {
                    emNoNode.SetActive(true);
                    return;
                    //AddNewNode(n2ID);
                    //DrawNode(graph.getNodes()[n2ID]);
                }
                AddNewEdgeToGraph(n1ID, n2ID);
            }
        }
        else
        {
            graph = new Graph();
            goEdges = new List<GameObject>();
            goNodes = new List<GameObject>();
            AddNewNodeToGraph();
        }
    }

    public void AddNewEdgeToGraph()
    {
        int value;
        if (int.TryParse(ifFrom.GetComponent<Text>().text, out value) &&
            int.TryParse(ifTo.GetComponent<Text>().text, out value))
        {
            AddNewEdgeToGraph(int.Parse(ifFrom.GetComponent<Text>().text), int.Parse(ifTo.GetComponent<Text>().text));
        } else
        {
            emNoNode.SetActive(true);
            return;
        }
        
    }

    public void AddNewEdgeToGraph(int node1ID, int node2ID)
    {
        if (graph != null)
        {
            if (graph.getNodes().Count > node2ID)
            {
                if (graph.getNodes()[node2ID] != null)
                {
                    Node n1 = graph.getNodes()[node1ID];
                    Node n2 = graph.getNodes()[node2ID];
                    Edge e = CreateEdge(n1, n2);
                    AddNewEdge(e);
                    DrawEdge(e);
                    return;
                }
            }
        }
        emNoNode.SetActive(true);
    }

    public void CreateGraph()
    {
        if (graph != null)
        {
            DeleteGraph();
        }
        
        graph = new Graph();
        goEdges = new List<GameObject>();
        goNodes = new List<GameObject>();
        if (pathToEdges == null || pathToNodes == null)
        {
            emNoPaths.SetActive(true);
            return;
        }

        fm = new FileManager(pathToNodes, pathToEdges);
        if (!fm.readNodes(graph) || !fm.readEdges(graph))
        {
            emWrongFormat.SetActive(true);
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
            newZ.z = distance / 2;
            newEdge.transform.localScale = newZ;
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
    
    public void DeleteGraph()
    {
        graph.DeleteEdges();
        graph.DeleteNodes();
        foreach (var goEdge in goEdges)
        {
            Destroy(goEdge);
        }

        foreach (var goNode in goNodes)
        {
            Destroy(goNode);
        }

        graph = null;
        goEdges = null;
        goNodes = null;
    }
}