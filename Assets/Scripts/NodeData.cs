using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeData : MonoBehaviour
{
    private List<NodeData> iNodes = new List<NodeData>();
    private List<EdgeData> iEdges = new List<EdgeData>();
    private int id;
    private string gender;
    private string age;
    private Vector3 vector3;
    private Vector3 forceVector3;
    private string region;
    private int degree;
    private int iDegree;
    private int oDegree;
    private bool cluster;
    private bool alredyShowed = false;
    private bool show = false;
    private Color defColor = new Color(0.0F, 0.0F, 255.0F, 0.0f);
    [SerializeField] private Renderer myRenderer;
    private bool find;

    public int ID
    {
        get => id;
        set => id = value;
    }

    public Vector3 Vector3
    {
        get => vector3;
        set => vector3 = value;
    }

    public Vector3 ForceVector3
    {
        get => forceVector3;
        set => forceVector3 = value;
    }

    public string Region
    {
        get => region;
        set => region = value;
    }

    public string Gender
    {
        get => gender;
        set => gender = value;
    }

    public string Age
    {
        get => age;
        set => age = value;
    }

    public bool Cluster
    {
        get => cluster;
        set => cluster = value;
    }
    
    public bool AlredyShowed
    {
        get => alredyShowed;
        set => alredyShowed = value;
    }

    public bool Show
    {
        get => show;
        set => show = value;
    }
    
    public bool Find
    {
        get => find;
        set => find = value;
    }
    
    public int Degree
    {
        get => degree;
        set => degree = value;
    }
    
    public int IDegree
    {
        get => iDegree;
        set => iDegree = value;
    }

    public int ODegree
    {
        get => oDegree;
        set => oDegree = value;
    }

    public List<NodeData> getiNodes()
    {
        return iNodes;
    }
    
    public List<EdgeData> getiEdges()
    {
        return iEdges;
    }

    /*public void setVector3(float x, float y, float z)
    {
        vector3.x = x;
        vector3.y = y;
        vector3.z = z;
    }*/
    
    public void SetForceVector3(float x, float y, float z)
    {
        forceVector3.x = x;
        forceVector3.y = y;
        forceVector3.z = z;
    }

    public void DeactiveIEdges()
    {
        foreach (var edge in iEdges)
        {
            edge.gameObject.SetActive(false);
            edge.Show = false;
        }
    }
    
    public void ActiveIEdges()
    {
        foreach (var edge in iEdges)
        {
            if (edge.Node1.isActiveAndEnabled && edge.Node2.isActiveAndEnabled)
            {
                edge.gameObject.SetActive(true);
                edge.Show = true;
            }
            else
            {
                edge.gameObject.SetActive(false);
                edge.Show = false;
            }
        }
    }

    public void NeighborsOn()
    {
        foreach (var e in iEdges)
        {
            e.OnClick();
        }

        foreach (var n in iNodes)
        {
            n.OnClick();
        }
    }

    public void NeighborsOff()
    {
        foreach (var e in iEdges)
        {
            e.OffClick();
        }

        foreach (var n in iNodes)
        {
            n.OffClick();
        }
    }

    public void ChangeColor(int s)
    {
        switch (s)
        {
            case 0: 
                myRenderer.material.SetColor("_EmissionColor", defColor); //default color
                myRenderer.material.EnableKeyword("_EmissionColor");
                break;
            case 1:
                myRenderer.material.SetColor("_EmissionColor", new Color(225.0F, 225.0F, 0.0F, 0.0F)); //yellow
                myRenderer.material.EnableKeyword("_EmissionColor");
                break;
            case 2:
                myRenderer.material.SetColor("_EmissionColor", new Color(0.0F, 255.0F, 0.0F, 0.0F)); //green
                myRenderer.material.EnableKeyword("_EmissionColor");
                break;
        }
    }

    public void OnClick()
    {
        ChangeColor(1);
    }

    public void OffClick()
    {
        if (!find)
        {
            ChangeColor(0);
        }
        else
        {
            ChangeColor(2);
        }
    }

    public void Finded()
    {
        ChangeColor(2);
        find = true;
    }

    /*public void resetDegree()
    {
        degree = iNodes.Count;
    }*/
    
    public void ResetODegree()
    {
        int tmp = 0;
        foreach (var e in iEdges)
        {
            if (e.Node1 == this || e.Bilateral)
            {
                tmp++;
            }
        }

        oDegree = tmp;
    }

    public void ResetIDegree()
    {
        int tmp = 0;
        foreach (var e in iEdges)
        {
            if (e.Node2 == this || e.Bilateral)
            {
                tmp++;
            }
        }

        iDegree = tmp;
    }
}
