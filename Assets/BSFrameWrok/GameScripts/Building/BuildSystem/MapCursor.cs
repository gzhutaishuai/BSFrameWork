using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCursor : MonoBehaviour
{
    public MartixMapRender mapRender;

    public GameObject createTower;//要创建的建筑物

    public Vector2 mousePosition;//鼠标位置

    public GameObject buildManager;

    private void Awake()
    {
        buildManager = GameObject.Find("BuildingManager").gameObject;
    }

    private void Update()
    {
        Ray ray=Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x,Input.mousePosition.y));

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            mousePosition.x = hit.point.x;
            mousePosition.y = hit.point.z;

            if (createTower != null)
            {
                Vector2 position2D=mapRender.GetPositionMartix(mousePosition.x,mousePosition.y);

                createTower.transform.position = new Vector3(position2D.x,0.5f,position2D.y);//建筑物创建位置


                if (Input.GetMouseButtonDown(0))
                {
                    if (mapRender.createTower)
                    {
                        for (int i = 0; i < mapRender.radiusObjects.Length; i++)
                        {
                            mapRender.points[mapRender.radiusObjects[i].GetComponent<TowerInfo>().Index].canCreate = false;
                            Destroy(mapRender.radiusObjects[i], 0);
                        }
                        createTower.transform.SetParent(buildManager.transform, true);
                        BuildingManager.Instance.buildings.Add(createTower.GetComponent<Building>());
                        createTower = null;
                    }
                    else
                    {
                        Debug.Log("空间不足，无法建造");
                    }
                }
            }
        }
    }

}
