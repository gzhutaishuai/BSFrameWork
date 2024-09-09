using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MartixMapRender : MonoBehaviour
{
    public static Vector2 sizeCube = new Vector2(1.6f, 1.6f);//点之间的间隔
    public Vector2 Wnums = new Vector2(35, 35);//点数量
    public List<pointMap> points = new List<pointMap>();//模拟地图的点

    public Vector2[] shemeRedius;//相对于炮塔的区域预制体位置
    public GameObject bottomPlane;//区域预制体
    public GameObject[] radiusObjects;//区域预制体组

    public bool createTower = false;

    public Material activeRegion;
    public Material noactiveRegion;

    private void Start()
    {
        //生成模拟地图
        for (int x = 0; x < Wnums.x; x++)
        {
            for (int y = 0; y < Wnums.y; y++)
            {
                pointMap p = new pointMap(x * sizeCube.x, y * sizeCube.y, (x * sizeCube.x) + sizeCube.x, (y * sizeCube.y) + sizeCube.y);
                points.Add(p);
            }
        }
    }

    //public void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    for (int x = 0; x < Wnums.x; x++)
    //    {
    //        for (int y = 0; y < Wnums.y; y++)
    //        {
    //            Gizmos.DrawSphere(new Vector3(x * sizeCube.x, 0, y * sizeCube.y), 1f);

    //        }
    //    }
    //}

    public void RefreshPalne()
    {
        radiusObjects = new GameObject[shemeRedius.Length];
        //Debug.Log("底座个数" + shemeRedius.Length);
        for (int i = 0; i < shemeRedius.Length; i++)
        {
            radiusObjects[i] = Instantiate(bottomPlane, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        }
    }

    public Vector2 GetPositionMartix(float x, float y)
    {
        Vector2 returnVector = new Vector2();

        for (int i = 0; i < points.Count; i++)
        {
            var point = points[i];

            float yCalc = (point.Y - point.sizeY) / 2;
            float xCalc = (point.X - point.sizeX) / 2;

            if (point.X + xCalc < x)
            {
                if ((point.X + point.sizeX) + xCalc > x)
                {
                    if (point.Y + yCalc < y)
                    {
                        if ((point.Y + point.sizeY) + yCalc > y)
                        {
                            returnVector.x = point.X;

                            returnVector.y = point.Y;

                            createTower = true;

                            RenderMartixToMap(i);
                        }
                    }
                }
            }
        }
        return returnVector;    
    }

    public void RenderMartixToMap(int index)
    {
        int indexEnd = 0;

        try
        {
            //底面图
            for(int i=0;i<shemeRedius.Length;i++)
            {
                Vector2 sRedius = shemeRedius[i];
                indexEnd = index;

                //Y
                if(sRedius.y<0)
                {
                    indexEnd = index + (Mathf.Abs((int)sRedius.y) * (int)Wnums.y);
                }
                else if(sRedius.y>0)
                {
                    indexEnd = index - (Mathf.Abs((int)sRedius.y) * (int)Wnums.y);
                }

                //X
                if (sRedius.x < 0)
                {
                    indexEnd +=Mathf.Abs((int)sRedius.x);
                }
                else if (sRedius.x > 0)
                {
                    indexEnd -= Mathf.Abs((int)sRedius.x);
                }


                //if empty region
                if (points[indexEnd].canCreate)
                {
                    radiusObjects[i].transform.GetComponent<Renderer>().material = activeRegion;
                }
                else
                {
                    radiusObjects[i].transform.GetComponent<Renderer>().material = noactiveRegion;
                    createTower = false;
                }
                radiusObjects[i].transform.position = new Vector3(points[indexEnd].X, 0.65f, points[indexEnd].Y);//小底座位置
                radiusObjects[i].GetComponent<TowerInfo>().Index = indexEnd;
            }
        }
        catch(UnityException e) 
        {
            Debug.Log("Error:" + e.Message);
        }
    }
}

public class pointMap
{
    public float X;
    public float Y;

    public float sizeX;
    public float sizeY;

    public bool canCreate;

    public pointMap(float x, float y, float sizeX, float sizeY)
    {
        this.X = x;
        this.Y = y;
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        canCreate = true;
    }
}
