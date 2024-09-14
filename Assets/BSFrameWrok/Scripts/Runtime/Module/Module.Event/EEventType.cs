using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSGame.Event
{
    public enum EEventType
    {
        //采集
        Attack_Event,

        //刷新资源UI
        Refresh_ResourcesUI,

        //重新获取所有资源状态
        Update_ResState,

        //刷新当前人口/最大人口UI
        Update_population
    }
}
