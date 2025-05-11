using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlantCollection : MonoBehaviour
{
    [SerializeReference]
    public List<Plant> plants=new List<Plant>();
    void Start()
    {
        
    }
}
