using Core.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        UIManager.Init();
        UIManager.Open<UIPanelInventory>();
    }
}
