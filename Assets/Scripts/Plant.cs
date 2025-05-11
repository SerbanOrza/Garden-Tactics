using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Plant 
{
    public string name;
    public List<Plant> veciniBuni=new List<Plant>();//should be unique names
    public List<int> friendshipValue=new List<int>();//this is for the veciniBuni values.
   // public List<Plant> veciniNeutri=new List<Plant>();

    public List<Plant> veciniRai=new List<Plant>();
    public Plant(string name)
    {
        this.name=name;
        this.veciniBuni=new List<Plant>();
        //this.veciniNeutri=new List<Plant>();
        this.veciniRai=new List<Plant>();
    }
    public Plant(string name,List<Plant> veciniBuni,List<int> friendshipValue,List<Plant> veciniRai)
    {
        this.name=name;
        this.veciniBuni=veciniBuni;
        this.friendshipValue=friendshipValue;
        this.veciniRai=veciniRai;
    }
    public Plant(string name,List<Plant> veciniBuni,List<Plant> veciniRai)
    {
        this.name=name;
        this.veciniBuni=veciniBuni;
        //this.veciniNeutri=new List<Plant>();
        this.friendshipValue=new List<int>(veciniBuni.Count);
        this.veciniRai=veciniRai;
    }
    // public int addFactorFromName(string name,List<int>values)
    // {
    //     int i=veciniBuni.FindIndex(x=>x.name==name);
    //     friendshipValue.Add()
    // }
    // public Plant(string name,List<Plant> veciniBuni,List<Plant> veciniNeutri,List<Plant> veciniRai)
    // {
    //     this.name=name;
    //     this.veciniBuni=veciniBuni;
    //     this.veciniNeutri=veciniNeutri;
    //     this.veciniRai=veciniRai;
    // }
    
}
