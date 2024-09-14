using Core.UI;
using RTSGame.Event;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[UILayer(UIPanelLayer.Battle)]
public class UIPanelInventory : UIPanelBase
{
    private TextMeshProUGUI txtFoodNum;
    private TextMeshProUGUI txtWoodNum;
    private TextMeshProUGUI txtStoneNum;
    private TextMeshProUGUI txtGoldNum;

    private TextMeshProUGUI txtworkFood;
    private TextMeshProUGUI txtworkMeat;
    private TextMeshProUGUI txtworkStone;
    private TextMeshProUGUI txtworkGold;

    private TextMeshProUGUI txtTolPeople;

    int hasActor;
    int maxActor;

    public override void OnUIAwake()
    {
        base.OnUIAwake();
        txtFoodNum = transform.Find("bottomBg/Imgmeat/txtCount").GetComponent<TextMeshProUGUI>();
        txtWoodNum = transform.Find("bottomBg/Imgwood/txtCount").GetComponent<TextMeshProUGUI>();
        txtStoneNum = transform.Find("bottomBg/Imgstone/txtCount").GetComponent<TextMeshProUGUI>();
        txtGoldNum = transform.Find("bottomBg/Imggold/txtCount").GetComponent<TextMeshProUGUI>();

        txtworkFood = transform.Find("bottomBg/Imgmeat/Imgfarmer/txtworkCount").GetComponent<TextMeshProUGUI>();
        txtworkMeat = transform.Find("bottomBg/Imgwood/Imgfarmer/txtworkCount").GetComponent<TextMeshProUGUI>();
        txtworkStone = transform.Find("bottomBg/Imgstone/Imgfarmer/txtworkCount").GetComponent<TextMeshProUGUI>();
        txtworkGold = transform.Find("bottomBg/Imggold/Imgfarmer/txtworkCount").GetComponent<TextMeshProUGUI>();

        txtTolPeople = transform.Find("upBg/ImgTolPeople/txtTolPeople").GetComponent<TextMeshProUGUI>();
    }

    public override void OnUIDestory()
    {
        base.OnUIDestory();
    }

    public override void OnUIDisable()
    {
        base.OnUIDisable();
        EventManager.Ignore(EEventType.Refresh_ResourcesUI, ReFreshResourceUI);
        EventManager.Ignore(EEventType.Update_population, ReFreshPopulationUI);
    }

    public override void OnUIEnable()
    {
        base.OnUIEnable();
        EventManager.Listen(EEventType.Refresh_ResourcesUI, ReFreshResourceUI);
        EventManager.Listen(EEventType.Update_population, ReFreshPopulationUI);
    }

    public override void OnUIStart()
    {
        base.OnUIStart();
    }

    public override void SetData(object data)
    {
        base.SetData(data);
    }

    /// <summary>
    /// Ë¢ÐÂ×ÊÔ´UI
    /// </summary>
    /// <param name="obj"></param>
    private void ReFreshResourceUI(params object[] obj)
    {
        List<ResourcesCount> resourcesCount = (List<ResourcesCount>)obj[0];
        for (int i = 0; i < resourcesCount.Count; i++)
        {
            switch (resourcesCount[i].type)
            {
                case Resources_Type.Wood:
                    txtWoodNum.text = resourcesCount[i].count.ToString();
                    break;
                case Resources_Type.Stone:
                    txtStoneNum.text = resourcesCount[i].count.ToString();
                    break;
                case Resources_Type.Food:
                    txtFoodNum.text = resourcesCount[i].count.ToString();
                    break;
                case Resources_Type.Gold:
                    txtGoldNum.text = resourcesCount[i].count.ToString();
                    break;
                default:
                    break;
            }
        }
    }

    private void ReFreshPopulationUI(params object[] obj)
    {
        hasActor = ActorManager.curActorsCount += (int)obj[0];
        maxActor = ActorManager.MaxActorsCount += (int)obj[1];
        
        if(hasActor>=maxActor)
        {
            txtTolPeople.text="<color=red>"+maxActor.ToString()+"/"+maxActor.ToString()+"</color>";
            ActorManager.curActorsCount =maxActor;
        }
        else
        {
            txtTolPeople.text = hasActor.ToString() + "/" + maxActor.ToString();
        }
        
        
    }
}
