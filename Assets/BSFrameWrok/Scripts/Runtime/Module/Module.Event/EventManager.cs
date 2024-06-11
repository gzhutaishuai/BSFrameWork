using System;

namespace GameFW.Event
{
    public static class EventManager 
    {
        public static void Listen(string eve,Action<object[]> callback)
        {
            BS.EventCenter.Listen(eve, callback);
        }

        public static void Ignore(string eve,Action<object[]> callback)
        {
            BS.EventCenter.Ignore(eve, callback);
        }
        public static void Trigger(string eve,params object[] args)
        {
            BS.EventCenter.Trigger(eve,args);
        }
        public static void Post(string eve,params object[] args)
        {
            BS.EventCenter.Post(eve, args);
        }
        public static void Listen(EEventType eve,Action<object[]> callback)
        {
            BS.EventCenter.Listen(eve.ToString(), callback);
        }
        public static void Ignore(EEventType eve,Action<object[]> callback)
        {
            BS.EventCenter.Ignore(eve.ToString(), callback);
        }
        public static void Trigger(EEventType eve,params object[] args)
        {
            BS.EventCenter.Trigger(eve.ToString(), args);
        }
        public static void Post(EEventType eve,params object[] args)
        {
            BS.EventCenter.Post(eve.ToString(), args);
        }
    }
}
