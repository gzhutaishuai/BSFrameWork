using System;
using System.Collections.Generic;
using UnityEngine;
namespace BSGame
{
    public class EventCenter : MonoBehaviour
    {

        private Dictionary<string, Action<object[]>> events = new Dictionary<string, Action<object[]>>();//所有事件集合

        class EventInfo
        {
            public string eventName;//事件名称
            public object[] args;//事件参数
        }

        private List<EventInfo> addToInfo = new List<EventInfo>();//新增事件
        private List<EventInfo> eventInfo = new List<EventInfo>();//已添加的所有事件

        //单例
        static EventCenter _instance;
        public static EventCenter insatance
        {
            get
            {
                return _instance;
            }
        }

         //监听消息
         public static void Listen(string evet,Action<object[]> callback)
         {
            //不包含此事件，就添加
            if(!insatance.events.ContainsKey(evet))
            {
                insatance.events[evet] = callback;
            }
            else
            {
                insatance.events[evet] -= callback;
                insatance.events[evet] += callback;
            }
         }
         //取消监听 
         public static void Ignore(string evet,Action<object[]> callback)
         {
            if(insatance.events.ContainsKey(evet))
            {
                insatance.events[evet] -= callback;
            }
         }
         //同步推送
         public static void Trigger(string evet,params object[] args)
         {
               if(insatance.events.TryGetValue(evet,out Action<object[]> callback)&&callback!=null)
               {
                   try
                   {
                       callback(args);
                   }
                   catch(Exception e)
                   {
                       Debug.Log(e.ToString());
                   }
               }
         }
        //延迟推送事件
        public static void Post(string evet,params object[] args)
        {
            EventInfo it = new EventInfo();
            it.eventName = evet;
            it.args = args;
            //保证不会出现并发修改的情况发生
            //避免出现一个线程正在添加一个事件至列表，同时另一个事件也在尝试添加
            lock(insatance.addToInfo)
            {
                insatance.addToInfo.Add(it);
            }
        }

        //每帧执行一次 后续考虑Mono
        private void Update()
        {
            if(eventInfo.Count>0)
            {
                for(int i=0,len=eventInfo.Count;i<len;i++)
                {
                    EventInfo item = eventInfo[i];
                    try
                    {
                        if(events.TryGetValue(item.eventName,out Action<object[]> callback)&&callback!=null)
                        {
                            callback(item.args);
                        }
                    }
                    catch(Exception e)
                    {
                        Debug.Log("EventCenter expection");
                    }
                }
                eventInfo.Clear();
            }
            lock(addToInfo)
            {
                if(addToInfo.Count>0)
                {
                    eventInfo.AddRange(addToInfo);
                    addToInfo.Clear();
                }
            }
        }
        
        /// <summary>
        /// 清空所有消息
        /// </summary>
        public void ClearAll()
        {
            events.Clear();
        }
    }
}
