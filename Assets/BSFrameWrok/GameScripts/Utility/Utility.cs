using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    public static RaycastHit ClickToDo()
    {
        Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        Vector3 position=Vector3.zero;
        if(Physics.Raycast(ray,out hitInfo,1000))
        { 
            return hitInfo;
        }
        return hitInfo;
    }
}
