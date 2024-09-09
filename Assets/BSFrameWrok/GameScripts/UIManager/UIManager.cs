using RTSGame.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    private void OnGUI()
    {
        if(GUI.Button(new Rect(500,20,110,30),"Create Tower 1"))
        {
            GameObject.Find("MapCursor_Settings").GetComponent<Tower>().createTower("_tower1");
        }
        if (GUI.Button(new Rect(500, 80, 110, 30), "Create Tower 2"))
        {
            this.SendMessage("createTower", "_tower2");
        }
        if (GUI.Button(new Rect(500, 140, 110, 30), "Create Tower 3"))
        {
            this.SendMessage("createTower", "_tower3");
        }
    }
}
