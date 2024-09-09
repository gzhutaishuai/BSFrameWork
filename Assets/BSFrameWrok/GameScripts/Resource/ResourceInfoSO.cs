using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Resources_Type
{
    Wood,
    Food,
    Gold,
    Stone,
}

[CreateAssetMenu(menuName = "RTS/Resource Type", fileName = "New Resoucrce Type")]
public class ResourceInfoSO : ScriptableObject
{
    //public Image icon;

    public ResourceTypeValue resValue;

    public Resources_Type resType;



}
