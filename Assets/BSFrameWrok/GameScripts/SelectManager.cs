using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    RaycastHit hit;
    Outline outline;

    private void Update()
    {
        OutLineControll();
    }
    private void OutLineControll()
    {
        if(Input.GetMouseButtonDown(0))
        {
            hit=Utility.ClickToDo();
            if (hit.collider == null) return;
            if (!hit.collider.CompareTag("Terrain")&& !hit.collider.CompareTag("Actor"))
            {
                if(hit.collider.transform.TryGetComponent(out Outline line))
                {
                    if(outline!=null) outline.enabled = false;
                    line.enabled=!line.enabled;
                    outline = line;
                    //触发刷新信息界面UI的信息

                }
            }
            else
            {
                if (outline!=null)
                {
                    outline.enabled = false;
                    outline = null;
                }

            }
        }
    }
}
