using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Node
{
    public Plant plant;
    public int id;
    public bool used;
    [SerializeReference]
    public List<Node> neighbors;
    public HashSet<int> badNeighbors;
    public List<int> costs;
    public Node(Plant plant,int id)
    {
        this.plant=plant;
        this.id=id;
        this.used=false;
        this.neighbors=new List<Node>();
        this.badNeighbors = new HashSet<int>();
        this.costs=new List<int>();
    }
}
