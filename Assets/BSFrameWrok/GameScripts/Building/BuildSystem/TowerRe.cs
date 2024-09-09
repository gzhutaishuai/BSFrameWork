using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRe : MonoBehaviour
{
    public int typeRegion = 0;
    public Vector2[] returnMartix;

    public void Start()
    {
        if (typeRegion == 0)
        {
            returnMartix = StandardRegion();
        }
        else if (typeRegion == 1)
        {
            returnMartix=MidRegion();
        }
        else if(typeRegion == 2)
        {
            Debug.Log("test3");
        }
    }

    public Vector2[] StandardRegion()
    {
        Vector2[]  matrix=new Vector2[9];

        matrix[0]=new Vector2(-1, -1);
        matrix[1]=new Vector2(-1, 0);
        matrix[2]=new Vector2(-1, 1);
        matrix[3]=new Vector2(0, -1);
        matrix[4]=new Vector2(0, 1);
        matrix[5]=new Vector2(1, -1);
        matrix[6]=new Vector2(1, 0);
        matrix[7]=new Vector2(1, 1);
        matrix[8]=new Vector2(0, 0);

        return matrix;
    }

    public Vector2[] MidRegion()
    {
        Vector2[] matrix = new Vector2[15];

        matrix[0] = new Vector2(-2, -1);
        matrix[1] = new Vector2(-1, -1);
        matrix[2] = new Vector2(0, -1);
        matrix[3] = new Vector2(1, -1);
        matrix[4] = new Vector2(2, -1);

        matrix[5] = new Vector2(-2, 0);
        matrix[6] = new Vector2(-1, 0);
        matrix[7] = new Vector2(0, 0);
        matrix[8] = new Vector2(1, 0);
        matrix[9] = new Vector2(2, 0);
        matrix[10] = new Vector2(-2, 1);
        matrix[11] = new Vector2(-1, 1);
        matrix[12] = new Vector2(0, 1);
        matrix[13] = new Vector2(1, 1);
        matrix[14] = new Vector2(2, -1);

        return matrix;
    }
}
