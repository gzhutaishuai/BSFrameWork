using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCursor : MonoBehaviour
{
    public MartixMapRender mapRender;

    public GameObject createTower;//Ҫ�����Ľ�����

    public Vector2 mousePosition;//���λ��

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

                createTower.transform.position = new Vector3(position2D.x,0.5f,position2D.y);//�����ﴴ��λ��


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
                        Debug.Log("�ռ䲻�㣬�޷�����");
                    }
                }
            }
        }
    }

}
