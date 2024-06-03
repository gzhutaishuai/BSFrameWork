using System;
using System.Collections.Generic;
using UnityEngine;
namespace BSGame
{
    public class EventCenter : MonoBehaviour
    {

        private Dictionary<string, Action<object[]>> events = new Dictionary<string, Action<object[]>>();//�����¼�����

        class EventInfo
        {
            public string eventName;//�¼�����
            public object[] args;//�¼�����
        }

        private List<EventInfo> addToInfo = new List<EventInfo>();//�����¼�
        private List<EventInfo> eventInfo = new List<EventInfo>();//����ӵ������¼�

        //����
        static EventCenter _instance;
        public static EventCenter insatance
        {
            get
            {
                return _instance;
            }
        }

         //������Ϣ
         public static void Listen(string evet,Action<object[]> callback)
         {
            //���������¼��������
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
         //ȡ������ 
         public static void Ignore(string evet,Action<object[]> callback)
         {
            if(insatance.events.ContainsKey(evet))
            {
                insatance.events[evet] -= callback;
            }
         }
         //ͬ������
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
        //�ӳ������¼�
        public static void Post(string evet,params object[] args)
        {
            EventInfo it = new EventInfo();
            it.eventName = evet;
            it.args = args;
            //��֤������ֲ����޸ĵ��������
            //�������һ���߳��������һ���¼����б�ͬʱ��һ���¼�Ҳ�ڳ������
            lock(insatance.addToInfo)
            {
                insatance.addToInfo.Add(it);
            }
        }

        //ÿִ֡��һ�� ��������Mono
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
        /// ���������Ϣ
        /// </summary>
        public void ClearAll()
        {
            events.Clear();
        }
    }
}
