using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public List<GameObject> towers=new List<GameObject>();
    public MapCursor mapCursor;

    private GameObject tower;

    public void createTower(string name)
    {
        switch (name)
        {
            case "_tower1":
                tower = Instantiate(towers[0],new Vector3(0,2,0), Quaternion.identity) as GameObject ;
            break;
            case "_tower2":
                tower = Instantiate(towers[1], new Vector3(), Quaternion.identity) as GameObject;
                break;
            //case "_tower3":
            //    tower = Instantiate(towers[2], new Vector3(), Quaternion.identity) as GameObject;
            //    break;
        }

        if (tower != null)
        {
            //初始化占地区域
            tower.GetComponent<TowerRe>().Start();

            mapCursor.mapRender.shemeRedius = tower.GetComponent<TowerRe>().returnMartix;
            mapCursor.createTower = tower;
            //渲染占地面积
            mapCursor.mapRender.RefreshPalne();
        }
        else
        {
            Debug.Log("tower is null");
        }
    }
}
