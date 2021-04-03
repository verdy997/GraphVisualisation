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
    public GameObject edgeBil;
    public GameObject emNoNode;
    public int from = 0;
    public int to = 0;
    public GameObject ifFrom;
    public GameObject ifTo;
    public GameObject ifFirstN;


    //private FileManager fm;
    private List<GameObject> goEdges;
    private List<GameObject> goNodes;
    private Graph graph;
    private float maxDSquared = 50;
    private string neighbor;
    private Ray ray;
    private RaycastHit hit;
    private int control = 0;
    private bool stop = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (graph != null && !stop)
        {
            if ((goNodes.Count != 0) && (goEdges.Count != 0))
            {
                ForceDirect();
                ControlIteration();
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

    private void ControlIteration()
    {
        control++;
        if (control == 70)
        {
            stop = true;
        }
    }

    public void setStop(bool s)
    {
        stop = s;
    }

    public Graph getGraph()
    {
        return graph;
    }

    public List<GameObject> getEdges()
    {
        return goEdges;
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
        float x = rnd.Next(0, 1000) / 100;
        float y = rnd.Next(0, 1000) / 100;
        float z = rnd.Next(0, 1000) / 100;
        Vector3 v3 = new Vector3(x, y, z);
        Node n1 = new Node(id, v3, "null");
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
        if (n.Degree > 0)
        {
            newNode.transform.localScale = new Vector3((n.Degree / 1000f), (n.Degree / 1000f), (n.Degree / 1000f));
        }
        newNode.name = n.ID.ToString();
        goNodes.Add(newNode);
    }

    private void DrawEdge(Edge e)
    {
        GameObject newEdge;
        if (e.isBilateral())
        {
            newEdge = Instantiate(edgeBil, e.Vector3, Quaternion.Euler(0f, 0f, 0f));
            if (e.Weight > 0)
            {
                newEdge.transform.localScale = new Vector3((e.Weight / 1000f), (e.Weight / 1000f), 1);
            }
        }
        else
        {
            newEdge = Instantiate(edge, e.Vector3, Quaternion.Euler(0f, 0f, 0f));
            if (e.Weight > 0)
            {
                newEdge.transform.localScale = new Vector3((e.Weight / 1000f), (e.Weight / 1000f), 1);
            }
        }

        newEdge.name = "." + e.Node1.ID.ToString() + ".-." + e.Node2.ID.ToString() + ".";
        newEdge.transform.LookAt(e.Node2.Vector3);
        float distance = Vector3.Distance(e.Node1.Vector3, e.Node2.Vector3);
        Vector3 newZ = newEdge.transform.localScale;
        newZ.z = distance / 2;
        newEdge.transform.localScale = newZ;
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
        }
        else
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

    public void CreateGraph(Graph g)
    {
        if (graph != null)
        {
            DeleteGraph();
        }

        graph = g;
        goEdges = new List<GameObject>();
        goNodes = new List<GameObject>();

        GeographicGraph(graph);
        
        DrawGraph(graph);
    }

    public void DHYB_w(Graph g, double w)
    {
        Debug.Log(g.getEdges().Count);
        Random rnd = new Random();
        //delection of random vertex/edge (modified)
        foreach (var nodeG in g.getNodes())
        {
            int tmp = rnd.Next(0, 100);
            if ((double)(tmp / 100) < w && nodeG != null)
            {
                int nLength = nodeG.getiNodes().Count;
                for (int i = 0; i < nodeG.getiEdges().Count; i++)
                {
                    tmp = rnd.Next(0, 100);
                    if ((double)(tmp / 100) < w)
                    {
                        int posDel = g.getEdges().IndexOf(nodeG.getiEdges()[i]);
                        if (posDel >= 0 && (g.getEdges()[posDel].Node1.getiEdges().Count > 1 &&
                                            g.getEdges()[posDel].Node2.getiEdges().Count > 1))
                        {
                            Edge re = g.getEdges()[posDel];
                            g.getEdges()[posDel].Node1.getiNodes().Remove(g.getEdges()[posDel].Node2);
                            g.getEdges()[posDel].Node2.getiNodes().Remove(g.getEdges()[posDel].Node1);
                            g.getEdges()[posDel].Node1.getiEdges().Remove(re);
                            g.getEdges()[posDel].Node2.getiEdges().Remove(re);
                            g.getEdges().RemoveAt(posDel);
                        }
                    }
                }
            }
        }

        if (g.getEdges().Count > g.getNodes().Count*2)
        {
            foreach (var edgeG in g.getEdges().ToList())
            {
                int tmp = rnd.Next(0, 100);
                if ((double)(tmp/100) < (1 - w))
                {
                    if (edgeG.Node1.getiEdges().Count > 1 && edgeG.Node2.getiEdges().Count > 1 )
                    {
                        edgeG.Node1.getiNodes().Remove(edgeG.Node2);
                        edgeG.Node2.getiNodes().Remove(edgeG.Node1);
                        edgeG.Node1.getiEdges().Remove(edgeG);
                        edgeG.Node2.getiEdges().Remove(edgeG);
                        g.getEdges().Remove(edgeG);
                    }
                }
            }
        }
        
        Debug.Log(g.getEdges().Count);
    }

    public void GeographicGraph(Graph g)
    {
        List<string> regions = new List<string>();
        List<int> density = new List<int>();

        foreach (var gnode in g.getNodes())
        {
            if (gnode != null)
            {
                if (!regions.Contains(gnode.Region))
                {
                    regions.Add(gnode.Region);
                    density.Add(1);
                }
                else
                {
                    int pos = regions.IndexOf(gnode.Region);
                    int newDen = density[pos];
                    newDen++;
                    density[pos] = newDen;
                }
            }
        }

        int[,] adjMatrix = new int[regions.Count,regions.Count];
        foreach (var gedge in g.getEdges())
        {
            int pos1 = regions.IndexOf(gedge.Node1.Region);
            int pos2 = regions.IndexOf(gedge.Node2.Region);
            adjMatrix[pos1,pos2] += 1;
        }

        Graph geoGraph = new Graph();
        for (int i = 0; i < regions.Count; i++)
        {
            Random rnd = new Random();
            float x = rnd.Next(0, 9000)/100;
            float y = rnd.Next(0, 9000)/100;
            float z = rnd.Next(0, 9000)/100;
            Vector3 vector3 = new Vector3(x, y, z);
            Node n1 = new Node(i, vector3, regions[i]);
            n1.Degree = density[i];
            geoGraph.AddNode(n1);
        }

        for (int i = 0; i < regions.Count; i++)
        {
            for (int j = 0; j < regions.Count; j++)
            {
                if (geoGraph.getNodes()[i] != geoGraph.getNodes()[j])
                {
                    Node n1 = geoGraph.getNodes()[i];
                    Node n2 = geoGraph.getNodes()[j];
                    if (n1.getiNodes().Contains(n2))
                    {
                        for (int k = 0; k < n1.getiEdges().Count; k++)
                        {
                            if (n1.getiEdges()[k].Node1 == n2)
                            {
                                n1.getiEdges()[k].Weight += adjMatrix[j, i];
                                n1.getiEdges()[k].setBilateral(true);
                                break;
                            }
                            else if (n1.getiEdges()[k].Node2 == n2)
                            {
                                n1.getiEdges()[k].Weight += adjMatrix[i, j];
                                n1.getiEdges()[k].setBilateral(true);
                                break;
                            }
                        }
                    }
                    else
                    {
                        n1.AddiNode(n2);
                        n2.AddiNode(n1);
                        Edge e = new Edge(n1, n2, n1.Vector3);
                        geoGraph.AddEdge(e);
                        n1.AddiEdge(e);
                        n2.AddiEdge(e);
                    }
                }
            }
        }

        graph = geoGraph;
        //DHYB_w(graph, 0.6);
        
    }

    public void DrawGraph(Graph g)
    {
        foreach (Node nodeInc in g.getNodes())
        {
            if (nodeInc != null)
            {
                DrawNode(nodeInc);
            }
            else
            {
                goNodes.Add(null);
            }
        }

        foreach (Edge edgeInc in g.getEdges())
        {
            DrawEdge(edgeInc);
        }
    }

    void ForceDirect()
    {
        int _L = 100; //spring rest length 200
        int _K_r = 5000; //repulsive force constant 200
        float _K_s = 1; //spring constant 1
        float delta_t = 0.05f; //time step 0.005
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
                    int max;
                    if (Mathf.Min(node1.getiNodes().Count, node2.getiNodes().Count) > 50)
                    {
                        max = 50;
                    }
                    else
                    {
                        max = Mathf.Min(node1.getiNodes().Count, node2.getiNodes().Count);
                    }

                    float distanceSquared = dv3.x * dv3.x + dv3.y * dv3.y + dv3.z * dv3.z;
                    float distance = Mathf.Sqrt(distanceSquared);
                    float force = (_K_r * max) / distanceSquared;
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