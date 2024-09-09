using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BS
{
    public delegate void TimerCallback(object param);
    public class Timers
    {
        public static int repeat;
        public static float time;

        //决定是否捕获定时任务回调函数中的异常 
        //true 如果有异常出现，将会被捕获并打印出异常信息，而不会将异常抛给调用者
        //false 有异常抛出时，会直接抛给调用者，不做任何处理
        public static bool catchCallbackExceptions = false;

        Dictionary<TimerCallback, Anymous_T> _items;
        Dictionary<TimerCallback, Anymous_T> _toAdd;
        List<Anymous_T> _toRemove;
        List<Anymous_T> _pool;

        TimersEngine _engine;
        GameObject gameObject;

        private static Timers _inst;
        public static Timers inst
        {
            get
            {
                if(_inst==null)
                {
                    _inst = new Timers();
                }
                return _inst;
            }
        }

        public Timers()
        {
            _inst = this;
            gameObject = new GameObject("[FairtGUI.Timers]");
            gameObject.hideFlags = HideFlags.HideInHierarchy;
            gameObject.SetActive(true);

            _engine = gameObject.AddComponent<TimersEngine>();

            _items = new Dictionary<TimerCallback, Anymous_T>();
            _toAdd = new Dictionary<TimerCallback, Anymous_T>();
            _toRemove = new List<Anymous_T>();
            _pool = new List<Anymous_T>(100);
        }

        public void Add(float interval,int repeat,TimerCallback callback)
        {
            Add(interval, repeat, callback,null);
        }

        public void Add(float interval,int repeat,TimerCallback callback,object callBackParam)
        {
            if(callback==null)
            {
                Debug.LogWarning("timer callback is null, " + interval + ","+repeat);
                return;
            }
            Anymous_T t;
            if(_items.TryGetValue(callback,out t))
            {
                t.set(interval, repeat, callback, callBackParam);
                t.elapsed = 0;
                t.deleted = false;
                return;
            }
            if(_toAdd.TryGetValue(callback,out t))
            {
                t.set(interval, repeat, callback, callBackParam);
                return;
            }
            t = GetFromPool();
            t.interval = interval;
            t.repeat = repeat;
            t.callback = callback;
            t.param = callBackParam;
            _toAdd[callback] = t;
        }

        public void CallLater(TimerCallback callback)
        {
            Add(0.001f, 1, callback);
        }

        public void CallLater(TimerCallback callback,object callBackParam)
        {
            Add(0.001f, 1, callback, callBackParam);
        }

        public void AddUpdate(TimerCallback callback)
        {
            Add(0.001f, 0, callback);
        }

        public void AddUpdate(TimerCallback callback,object callBackParam)
        {
            Add(0.001f, 0, callback,callBackParam);
        }
        
        //public void StartCoroutine(IEnumerator routine)
        //{
        //    _engine.StartCoroutine(routine);
        //}

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return _engine.StartCoroutine(routine);
        }

        public void StopCoroutine(Coroutine routine)
        {
            _engine.StopCoroutine(routine);
        }

        public bool Exists(TimerCallback callback)
        {
            if(_toAdd.ContainsKey(callback))
            {
                return true;
            }

            Anymous_T at;

            if(_items.TryGetValue(callback,out at))
            {
                return !at.deleted;
            }
            return false;
        }

        public void Remove(TimerCallback callback)
        {
            Anymous_T t;
            if(_toAdd.TryGetValue(callback,out t))
            {
                _toAdd.Remove(callback);
                ReturnToPool(t);
            }

            if (_items.TryGetValue(callback, out t))
                t.deleted = true;
        }
        private Anymous_T GetFromPool()
        {
            Anymous_T t;
            int cnt = _pool.Count;
            if (cnt > 0)
            {
                t = _pool[cnt - 1];
                _pool.RemoveAt(cnt - 1);
                t.deleted = false;
                t.elapsed = 0;
            }
            else
            {
                t = new Anymous_T();
            }
            return t;
        }

        private void ReturnToPool(Anymous_T t)
        {
            t.callback = null;
            _pool.Add(t);
        }
        public void Update()
        {
            float dt = Time.unscaledDeltaTime;//取得从上一帧到当前帧的时间差，单位为秒,Time.unscaledDeltaTime返回的是不受Time Scale影响的两帧之间的纯时间间隔
            Dictionary<TimerCallback, Anymous_T>.Enumerator iter;//枚举器,用于遍历正在运行的所有定时器的任务，从而可以对每一个定时器执行相关的操作

            if (_items.Count>0)
            {
                iter = _items.GetEnumerator();
                while(iter.MoveNext())
                {
                    Anymous_T i = iter.Current.Value;
                    if(i.deleted)
                    {
                        _toRemove.Add(i);
                    }

                    //未到计时时间
                    i.elapsed += dt;
                    if (i.elapsed < i.interval)
                        continue;

                    //已到计时时间
                    i.elapsed -= i.interval;
                    if(i.elapsed<0||i.elapsed>0.03f)
                    {
                        i.elapsed = 0;
                    }

                    //不无限重复
                    if(i.repeat>0)
                    {
                        i.repeat--;
                        if(i.repeat==0)
                        {
                            i.deleted = true;
                            _toRemove.Add(i);
                        }
                    }

                    repeat = i.repeat;

                    //抛异常
                    if(i.callback!=null)
                    {
                        if (catchCallbackExceptions)
                        {
                            try
                            {
                                i.callback(i.param);
                            }
                            catch (System.Exception e)
                            {
                                i.deleted = true;
                                Debug.LogWarning("FairyGUI: timer(internal=" + i.interval + ", repeat=" + i.repeat + ") callback error > " + e.Message);
                            }
                        }
                        else
                            //回调
                            i.callback(i.param);
                    }
                    iter.Dispose();
                }
            }
            //移除定时器监听
            int len = _toRemove.Count;
            if (len > 0)
            {
                for (int k = 0; k < len; k++)
                {
                    Anymous_T i = _toRemove[k];
                    if (i.deleted && i.callback != null)
                    {
                        _items.Remove(i.callback);
                        ReturnToPool(i);
                    }
                }
                _toRemove.Clear();
            }

            //最后才添加新的计时器监听
            if (_toAdd.Count > 0)
            {
                Debug.Log("添加新计时器");
                iter = _toAdd.GetEnumerator();
                while (iter.MoveNext())
                {
                    _items.Add(iter.Current.Key, iter.Current.Value);
                }
                iter.Dispose();
                _toAdd.Clear();
            }
        }
    }

    class Anymous_T
    {
        public float interval;//间隔
        public int repeat;//重复次数，为0则无限重复
        public TimerCallback callback;//回调函数
        public object param;//回调参数

        public float elapsed;//已过去的时间
        public bool deleted;//删除标志

        public void set(float interval,int repeat,TimerCallback callback,object param)
        {
            this.interval = interval;
            this.repeat = repeat;
            this.callback = callback;
            this.param = param;
        }
    }

    class TimersEngine:MonoBehaviour
    {
        private void Update()
        {
            Timers.inst.Update();
        }
    }
}