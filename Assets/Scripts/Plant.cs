using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Plant 
{
    public string name;
    public List<string> veciniBuni=new List<string>();//should be unique names
    public List<int> friendshipValues=new List<int>();//this is for the veciniBuni values.
   // public List<Plant> veciniNeutri=new List<Plant>();

    public List<string> veciniRai=new List<string>();
    public Plant(string name)
    {
        this.name=name;
        this.veciniBuni=new List<string>();
        //this.veciniNeutri=new List<Plant>();
        this.veciniRai=new List<string>();
    }
    public Plant(string name,List<Plant> veciniBuni,List<int> friendshipValues,List<Plant> veciniRai)
    {
        this.name=name;
        this.veciniBuni=convertToNames(veciniBuni);
        this.friendshipValues=friendshipValues;
        this.veciniRai=convertToNames(veciniRai);
    }
    // public Plant(string name,List<Plant> veciniBuni,List<Plant> veciniRai)
    // {
    //     this.name=name;
    //     this.veciniBuni=veciniBuni;
    //     //this.veciniNeutri=new List<Plant>();
    //     this.friendshipValues=new List<int>(veciniBuni.Count);
    //     this.veciniRai=veciniRai;
    // }
    public List<string> convertToNames(List<Plant>plants)
    {
        List<string> ans=new List<string>();
        foreach(Plant p in plants)
            ans.Add(p.name);
        return ans;
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
