using RTSGame.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourcesUI : MonoBehaviour
{

    private TextMeshProUGUI _woodNum;
    private TextMeshProUGUI _stoneNum;
    private TextMeshProUGUI _foodNum;
    private TextMeshProUGUI _goldNum;
    private void Awake()
    {
        _woodNum=transform.Find("ResCanvas/imgWood/txtNum").GetComponent<TextMeshProUGUI>();
        _stoneNum=transform.Find("ResCanvas/imgStone/txtNum").GetComponent<TextMeshProUGUI>();
        _foodNum=transform.Find("ResCanvas/imgFood/txtNum").GetComponent<TextMeshProUGUI>();
        _goldNum = transform.Find("ResCanvas/imgGold/txtNum").GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        EventManager.Listen(EEventType.Refresh_ResourcesUI, ReFreshResourceUI);
    }

    private void OnDisable()
    {
        EventManager.Ignore(EEventType.Refresh_ResourcesUI, ReFreshResourceUI);
    }

    private void ReFreshResourceUI(params object[] obj) 
    {
       List<ResourcesCount> resourcesCount = (List<ResourcesCount>)obj[0];
        for (int i = 0; i < resourcesCount.Count; i++)
        {
            switch (resourcesCount[i].type)
            {
                case Resources_Type.Wood:
                    _woodNum.text = resourcesCount[i].count.ToString();
                    break;
                case Resources_Type.Stone:
                    _stoneNum.text = resourcesCount[i].count.ToString(); 
                    break;
                case Resources_Type.Food:
                    _foodNum.text = resourcesCount[i].count.ToString(); 
                    break;
                case Resources_Type.Gold:
                    _goldNum.text = resourcesCount[i].count.ToString(); 
                    break;
                default:
                    break;
            }



        }
    }
}
