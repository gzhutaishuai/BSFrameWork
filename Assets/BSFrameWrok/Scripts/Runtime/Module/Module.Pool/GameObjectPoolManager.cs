using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameFW.Pool
{
    /// <summary>
    /// 游戏对象池管理器
    /// </summary>
    public sealed class GameObjectPoolManager : Singleton<GameObjectPoolManager>, IModule
    {
        public class CreateParamaters
        {
            /// <summary>
            /// 是否启用惰性对象池
            /// </summary>
            public bool IsLazyPool = false;

            /// <summary>
            /// 默认的初始容器值
            /// </summary>
            public int DefaultInitCapacity = 0;

            /// <summary>
            /// 默认的最大容器值
            /// </summary>
            public int DefaultMaxCapacity = int.MaxValue;

            /// <summary>
            ///  默认的静默销毁时间
            ///  小于0代表不主动销毁
            /// </summary>
            public float DefaultDestoryTime = -1f;
        }

        private readonly Dictionary<string, GameObjectCollector> _collectors = new Dictionary<string, GameObjectCollector>(100);//存储所有对象的集合，键是资源位置，值是对象集的引用
        private readonly List<GameObjectCollector> _removeList = new List<GameObjectCollector>(100);//存储要销毁的对象集合
        private GameObject _root;
        private bool _isLazyPool;
        private int _defaultInitCapacity;
        private int _defaultMaxCapacity;
        private float _defaultDestoryTime;


        public void OnCreate(object createParam)
        {
            // 检测依赖模块
            //if (BSEngine.Contains(typeof(ResourceManager)) == false)
            //    throw new Exception($"{nameof(GameObjectPoolManager)} depends on {nameof(ResourceManager)}");

            CreateParamaters parameters = createParam as CreateParamaters;

            if (parameters == null)
                throw new Exception($"{nameof(GameObjectPoolManager)} create param is invalid");
            if(parameters.DefaultMaxCapacity<parameters.DefaultInitCapacity)
                throw new Exception("The max capacity value must be greater the init capacity value.");

            _isLazyPool = parameters.IsLazyPool;
            _defaultInitCapacity = parameters.DefaultInitCapacity;
            _defaultMaxCapacity = parameters.DefaultMaxCapacity;
            _defaultDestoryTime = parameters.DefaultDestoryTime;

            _root = new GameObject("[PoolManager]");
            _root.transform.position = Vector3.zero;
            _root.transform.eulerAngles = Vector3.zero;
            UnityEngine.Object.DontDestroyOnLoad(_root);

        }

        public void OnDestroy()
        {
            
        }

        public void OnGUI()
        {
            
        }

        public void OnUpdate()
        {
            _removeList.Clear();
            foreach(var valuePair in _collectors)
            {
                var collector = valuePair.Value;
                if (collector.CanAutoDestory())
                    _removeList.Add(collector);
            }

            //移除并销毁
            foreach(var collector in _removeList)
            {
                _collectors.Remove(collector.Location);
                collector.Destory();
            }
        }

        void IModule.OnDestroy()
        {
            
        }


    }
}
