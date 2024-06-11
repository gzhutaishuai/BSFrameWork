using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFW.Pool
{
    public class SpawnGameobject
    {
        /// <summary>
        /// 生成状态
        /// </summary>
        internal enum ESpawnState
        {
            None=0,

            /// <summary>
            /// 已回收
            /// </summary>
            Restore=1,

            /// <summary>
            /// 已丢弃
            /// </summary>
            Discard=2,
        }

        private GameObjectCollector _cacheCollector;

        /// <summary>
        /// 判断属于回收还是释放状态
        /// </summary>
        internal ESpawnState SpawnState { private set; get; } = ESpawnState.None;

        /// <summary>
        /// 游戏对象
        /// </summary>
        public GameObject Go { internal set; get; }

        /// <summary>
        /// 用户自定义数据集
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
        /// 构造函数
        /// </summary>
        /// <param name="collector"></param>
        /// <param name="userDatas"></param>
        internal SpawnGameobject(GameObjectCollector collector,params System.Object[] userDatas)
        {
            _cacheCollector = collector;
            UserDatas = userDatas;
        }

        /// <summary>
        /// 构造函数
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
        /// 回收
        /// </summary>
        public void Restore()
        {
            UserCallback = null;
            SpawnState = ESpawnState.Restore;
            _cacheCollector.Restore(this);
        }

        /// <summary>
        /// 丢弃
        /// </summary>
         public void Discard()
        {
            UserCallback = null;
            SpawnState = ESpawnState.Discard;
            _cacheCollector.Discard(this);
        }


        #region 异步相关
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
