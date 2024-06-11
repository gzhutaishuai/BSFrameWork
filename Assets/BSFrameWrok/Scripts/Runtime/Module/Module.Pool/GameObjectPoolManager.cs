using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameFW.Pool
{
    /// <summary>
    /// ��Ϸ����ع�����
    /// </summary>
    public sealed class GameObjectPoolManager : Singleton<GameObjectPoolManager>, IModule
    {
        public class CreateParamaters
        {
            /// <summary>
            /// �Ƿ����ö��Զ����
            /// </summary>
            public bool IsLazyPool = false;

            /// <summary>
            /// Ĭ�ϵĳ�ʼ����ֵ
            /// </summary>
            public int DefaultInitCapacity = 0;

            /// <summary>
            /// Ĭ�ϵ��������ֵ
            /// </summary>
            public int DefaultMaxCapacity = int.MaxValue;

            /// <summary>
            ///  Ĭ�ϵľ�Ĭ����ʱ��
            ///  С��0������������
            /// </summary>
            public float DefaultDestoryTime = -1f;
        }

        private readonly Dictionary<string, GameObjectCollector> _collectors = new Dictionary<string, GameObjectCollector>(100);//�洢���ж���ļ��ϣ�������Դλ�ã�ֵ�Ƕ��󼯵�����
        private readonly List<GameObjectCollector> _removeList = new List<GameObjectCollector>(100);//�洢Ҫ���ٵĶ��󼯺�
        private GameObject _root;
        private bool _isLazyPool;
        private int _defaultInitCapacity;
        private int _defaultMaxCapacity;
        private float _defaultDestoryTime;


        public void OnCreate(object createParam)
        {
            // �������ģ��
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

            //�Ƴ�������
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
