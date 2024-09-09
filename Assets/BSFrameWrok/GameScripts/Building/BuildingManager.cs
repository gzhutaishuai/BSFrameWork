using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager :MonoSingleton<BuildingManager>
{
    //[HideInInspector]
    public List<Building> buildings = new List<Building>();

    private void Start()
    {
        buildings.AddRange(GetComponentsInChildren<Building>());
    }



}
