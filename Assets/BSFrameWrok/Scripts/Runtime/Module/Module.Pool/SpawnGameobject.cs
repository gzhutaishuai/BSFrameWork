using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFW.Pool
{
    public class SpawnGameobject
    {
        /// <summary>
        /// ����״̬
        /// </summary>
        internal enum ESpawnState
        {
            None=0,

            /// <summary>
            /// �ѻ���
            /// </summary>
            Restore=1,

            /// <summary>
            /// �Ѷ���
            /// </summary>
            Discard=2,
        }

        private GameObjectCollector _cacheCollector;

        /// <summary>
        /// �ж����ڻ��ջ����ͷ�״̬
        /// </summary>
        internal ESpawnState SpawnState { private set; get; } = ESpawnState.None;

        /// <summary>
        /// ��Ϸ����
        /// </summary>
        public GameObject Go { internal set; get; }

        /// <summary>
        /// �û��Զ������ݼ�
        /// </summary>
        public System.Object[] UserDatas { private set; get; }

        public System.Object UserData
        {
            get
            {
                if (UserDatas != null && UserDatas.Length > 0)
                    return UserDatas[0];
                else
                    return null;
            }
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="collector"></param>
        /// <param name="userDatas"></param>
        internal SpawnGameobject(GameObjectCollector collector,params System.Object[] userDatas)
        {
            _cacheCollector = collector;
            UserDatas = userDatas;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="collector"></param>
        /// <param name="go"></param>
        /// <param name="userDatas"></param>
        internal SpawnGameobject(GameObjectCollector collector,GameObject go,params System.Object[] userDatas)
        {
            _cacheCollector = collector;
            Go = go;
            UserDatas = userDatas;
        }

        /// <summary>
        /// ����
        /// </summary>
        public void Restore()
        {
            UserCallback = null;
            SpawnState = ESpawnState.Restore;
            _cacheCollector.Restore(this);
        }

        /// <summary>
        /// ����
        /// </summary>
         public void Discard()
        {
            UserCallback = null;
            SpawnState = ESpawnState.Discard;
            _cacheCollector.Discard(this);
        }


        #region �첽���
        internal System.Action<SpawnGameobject> UserCallback;

        public event System.Action<SpawnGameobject> Completed
        {
            add
            {
                if(Go!=null)
                {
                    value.Invoke(this);
                }
                else
                {
                    UserCallback += value;
                }
            }
            remove
            {
                UserCallback -= value;
            }
        }
        #endregion
    }
}
