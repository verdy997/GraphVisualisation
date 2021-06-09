using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class GUI_EL : MonoBehaviour
{
    [SerializeField] private GraphRepresentation gr;
    [SerializeField] private FileManager fm;
    [SerializeField] private GraphData graph;
    [SerializeField] private RegionGraph rGraph;

    [SerializeField] private GameObject nodeInfo;
    [SerializeField] private Text iDNode;
    [SerializeField] private Text region;
    [SerializeField] private Text profileInfo;
    
    [SerializeField] private GameObject emNoPaths;
    [SerializeField] private GameObject emWrongFormat;
    [SerializeField] private GameObject emWrongCalc;
    [SerializeField] private GameObject emNoNode;
    [SerializeField] private GameObject emNoDirectory;
    [SerializeField] private GameObject emNoGraph;
    
    [SerializeField] private Text fromN;
    [SerializeField] private Text toN;
    
    [SerializeField] private GameObject clusterInfo;
    [SerializeField] private Text keyCluster;
    [SerializeField] private Text iClusters;
    [SerializeField] private Text content;
    [SerializeField] private GameObject btnShow;
    [SerializeField] private GameObject btnHide;
    
    [SerializeField] private Text spring;
    [SerializeField] private Text force;
    [SerializeField] private Text nOR;
    [SerializeField] private Text timeS;
    
    [SerializeField] private Text infoGraph;
    
    [SerializeField] private Text projectName;
    
    [SerializeField] private Text fFrom;
    [SerializeField] private Text fTo;

    [SerializeField] private Explorer explorer;
    
    private string pathToEdges;
    private string pathToNodes;
    private string pathProject;
    private NodeData node;
    private bool edgesActive;
    private int findID;

    private int id;
    private string key;
    //
    
    public void ShowInfo(GameObject go)
    {
        if (node != null)
        {
            node.OffClick();
            foreach (var e in node.getiEdges())
            {
                e.OffClick();
            }
        }
        if (go.tag == "Node")
        {
            id = int.Parse(go.name);
            if (int.TryParse(fromN.text, out int value))
            {
                node = gr.GetGraph().getNodes()[(id-int.Parse(fromN.text))];
            }
            else
            {
                node = gr.GetGraph().getNodes()[id-1];
            }
           
            
            node.OnClick();
            foreach (var e in node.getiEdges())
            {
                e.OnClick();
            }
            iDNode.GetComponent<Text>().text = id.ToString();
            region.GetComponent<Text>().text = node.Region;
            string s = "Gender:\t" + node.Gender + "\nAge:\t" + node.Age + "\nDegree:\t" + node.Degree
                       + "\niDegree:\t" + node.IDegree + "\noDegree:\t" + node.ODegree;
            profileInfo.GetComponent<Text>().text = s;
            nodeInfo.SetActive(true);
        }
        else if (go.tag == "Cluster")
        {
            key = go.name;
            node = rGraph.getRegionNodes()[key];
            node.OnClick();
            string cont = "";
            foreach (var e in node.getiEdges())
            {
                e.OnClick();
                if (e.Node1.Region == key)
                {
                    if (e.Node2.Cluster)
                    {
                        cont += e.Node2.Region + " - " + e.Weight + "\n";
                    }
                }
                else
                {
                    if (e.Node1.Cluster)
                    {
                        cont += e.Node1.Region + " - " + e.Weight + "\n";
                    }
                }
            }
            if (!rGraph.getRegionNodes()[key].Show)
            {
                btnShow.gameObject.SetActive(false);
                btnHide.gameObject.SetActive(true);
            }
            else
            {
                btnShow.gameObject.SetActive(true);
                btnHide.gameObject.SetActive(false);
            }

            content.GetComponent<Text>().text = cont;

            string[] town = key.Split(',');
            keyCluster.GetComponent<Text>().text = town[1];
            iClusters.GetComponent<Text>().text = node.Degree.ToString();
            clusterInfo.SetActive(true);
        }
    }

    public void InfoGraphSet()
    {
        var nShow = graph.getNodes().Where(n => n.Show).Count();
        
        string s = graph.getNodes().Where(n => n.Show).Count() + "\n" +
                   graph.getEdges().Where(e => e.Show).Count() + "\n";
        infoGraph.GetComponent<Text>().text = s;
    }

    public void Close()
    {
        node.OffClick();
        foreach (var e in node.getiEdges())
        {
            e.OffClick();
        }
    }

    public void CalculatePositionDialogSequential()
    {
        if (isThereGraph())
        {
            int value;
            float fvalue;
            if (spring == null || force == null || nOR == null || timeS == null || 
                !(int.TryParse(spring.text, out value)) || !(int.TryParse(force.text, out value)) || 
                !(int.TryParse(nOR.text, out value)) || !(float.TryParse(timeS.text, out fvalue)))
            {
                emWrongCalc.gameObject.SetActive(true);
                return;
            }
            else
            {
                gr.CalculatePositionSequential(int.Parse(spring.text), int.Parse(force.text), 
                    int.Parse(nOR.text), float.Parse(timeS.text));
            }
        }
    }
    
    public void CalculatePositionDialogParallel()
    {
        if (isThereGraph())
        {
            int value;
            float fvalue;
            if (spring == null || force == null || nOR == null || timeS == null || 
                !(int.TryParse(spring.text, out value)) || !(int.TryParse(force.text, out value)) || 
                !(int.TryParse(nOR.text, out value)) || !(float.TryParse(timeS.text, out fvalue)))
            {
                emWrongCalc.gameObject.SetActive(true);
                return;
            }
            else
            {
                gr.CalculatePositionParallel(int.Parse(spring.text), int.Parse(force.text), 
                    int.Parse(nOR.text), float.Parse(timeS.text));
            }   
        }
    }

    public void FindRegion(Text infoCls)
    {
        if (rGraph.Active)
        {
            if (!rGraph.getRegionNodes().ContainsKey(infoCls.text))
            {
                emNoNode.gameObject.SetActive(true);
            }
            else
            {
                ShowInfo(rGraph.getRegionNodes()[infoCls.text].gameObject);
            }
        }
        else
        {
            
            var regNodes = graph.getNodes().Where(n => n.Region == infoCls.text).ToList();
            foreach (var n in regNodes)
            {
                n.Finded();
            }
        }
    }

    public void ShowCluster()
    {
        if (!rGraph.getRegionNodes()[key].AlredyShowed)
        {
            rGraph.ShowOne(key);
        }
        else
        {
            rGraph.ShowASOne(key);
        }
    }

    public void ShowNeighbors()
    {
        var n = graph.getNodes().Where(node => node.ID == id).Single();
        n.NeighborsOn();
    }

    public void HideNeighbors()
    {
        var n = graph.getNodes().Where(node => node.ID == id).Single();
        if (!edgesActive)
        {
            n.DeactiveIEdges();
        }
        n.NeighborsOff();
    }
    
    public void HideCluster()
    {
        rGraph.HideOne(key);
    }

    public void FindNodes()
    {
        pathToNodes = explorer.ShowExplorer();

    }

    public void FindEdges()
    {
        pathToEdges = explorer.ShowExplorer();
    }
    
    /*public void FindProject()
    {
        //pathProject = EditorUtility.OpenFolderPanel("Searching for project", "Projects", "");
        pathProject = explorer.FindFolder();
    }*/
    
    public void SetEdgeVisibility(bool value)
    {
        graph.ActiveEdges = value;
        graph.setEdgeVisibility(value);
        rGraph.ActiveEdges = value;
        rGraph.setEdgeVisibility(value);
        edgesActive = value;
        string s = graph.getNodes().Where(n => n.Show).Count() + "\n" +
                   graph.getEdges().Where(e => e.Show).Count() + "\n";
        infoGraph.GetComponent<Text>().text = s;
    }

    public void LoadData()
    {

        if (pathToEdges == null || pathToNodes == null)
        {
            emNoPaths.SetActive(true);
            return;
        }

        if (int.TryParse(toN.text, out int value))
        {
            if (!fm.ReadNodes(pathToNodes, int.Parse(toN.text)) || !fm.ReadEdges(pathToEdges, int.Parse(toN.text)))
            {
                emWrongFormat.SetActive(true);
                return;
            }
        }
        else
        {
            if (!fm.ReadNodes(pathToNodes, 0) || !fm.ReadEdges(pathToEdges, 0))
            {
                emWrongFormat.SetActive(true);
                return;
            }
        }
        
        if (fromN.text.Length == 0)
        {
            if (toN.text.Length == 0)
            {
                fm.SendData();
            }
            else
            {
                fm.SendData(1, int.Parse(toN.text));
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

    public void SaveData()
    {
        if (isThereGraph())
        {
            string name = projectName.GetComponent<Text>().text.ToString();
            fm.SaveData(name);
        }
    }

    /*public void LoadProject()
    {
        if (pathProject == null)
        {
            emNoDirectory.gameObject.SetActive(true);
            return;
        }
        string pathN = pathProject + "\\nodes.txt";
        string pathE = pathProject + "\\edges.txt";
        fm.ReadNodes(pathN, 0);
        fm.ReadEdges(pathE, 0);
        fm.SendData();
    }*/

    private bool isThereGraph()
    {
        if (graph.getNodes().Count == 0)
        {
            emNoGraph.gameObject.SetActive(true);
            return false;
        }

        return true;
    }
    
    public void ShowRegionGraph()
    {
        if (isThereGraph())
        {
            gr.CreateRegionGraph();
        }
    }

    public void ShowGraph()
    {
        if (isThereGraph())
        {
            gr.ShowGraph();
        }
    }

    public void SetFindID(int s)
    {
        switch (s)
        {
            case 0:
                findID = 0;
                break;
            case 1:
                findID = 1;
                break;
            case 2:
                findID = 2;
                break;
        }
    }

    public void DefaultColor()
    {
        foreach (var n in graph.getNodes())
        {
            n.Find = false;
            n.ChangeColor(0);
        }
    }
    
    public void FindBy()
    {
        if (isThereGraph())
        {
            int from;
            int to;
            if (int.TryParse(fFrom.text, out int value) || int.TryParse(fTo.text, out int value1))
            {
                from = int.Parse(fFrom.text);
                to = int.Parse(fTo.text);
            }
            else
            {
                return;
            }
        
        
            if (findID == 0)
            {
                var iDegNodes = graph.getNodes().Where(n => n.IDegree >= from && n.IDegree <= to).ToList();
                foreach (var n in iDegNodes)
                {
                    n.Finded();
                }

                return;
            } else if (findID == 1)
            {
                var oDegNodes = graph.getNodes().Where(n => n.ODegree >= from && n.ODegree <= to).ToList();
                foreach (var n in oDegNodes)
                {
                    n.Finded();
                }

                return;
            } else if (findID == 2)
            {
                var degNodes = graph.getNodes().Where(n => n.Degree >= from && n.Degree <= to).ToList();
                foreach (var n in degNodes)
                {
                    n.Finded();
                }

                return;
            }   
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
