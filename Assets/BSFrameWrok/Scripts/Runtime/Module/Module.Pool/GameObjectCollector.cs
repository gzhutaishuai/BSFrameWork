using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace GameFW.Pool
{
    public class GameObjectCollector : IEnumerator
    {
        private readonly Queue<GameObject> _cache;//已经加载进内存但未被使用的游戏对象实例
        private readonly List<SpawnGameobject> _loadingSpawn = new List<SpawnGameobject>();//正在加载但还未加载完成的游戏对象实例
        private readonly Transform _root;//所有已经加载并回收，但未使用的游戏对象的父节点
        private AssetHandle _handle;//资源操作句柄，获取资源加载的过程和结果
        private float _lastRestoreRealTime = -1f;//所有对象被归还到对象池的时间

        /// <summary>
        /// 资源定位地址
        /// </summary>
        public string Location { private set; get; }

        /// <summary>
        /// 资源常驻不销毁
        /// </summary>
        public bool DontDestory { private set; get; }

        /// <summary>
        /// 对象池的初始容量
        /// </summary>
        public int InitCapacity { private set; get; }

        /// <summary>
        /// 对象池的最大容量
        /// </summary>
        public int MaxCapacity { private set; get; }

        /// <summary>
        /// 静默销毁时间
        /// </summary>
        public float DestoryTime { private set; get; }

        /// <summary>
        /// 是否加载完毕
        /// </summary>
        public bool isDone
        {
            get
            {
                return _handle.IsDone;
            }
        }

        /// <summary>
        /// 当前加载状态
        /// </summary>
        public EOperationStatus States
        {
            get
            {
                return _handle.Status;
            }
        }

        /// <summary>
        /// 内部缓存总数
        /// </summary>
        public int CacheCount
        {
            get
            {
               return _cache.Count;
            }
        }

        /// <summary>
        /// 外部使用总数
        /// </summary>
        public int SpawnCount { private set; get; } = 0;

        /// <summary>
        /// 构造函数
        /// </summary>
        public GameObjectCollector(Transform root,string location,bool dontDestory,int initCapacity,int maxCapacity,float destoryTime)
        {
            _root = root;
            Location = location;
            DontDestory = dontDestory;
            InitCapacity = initCapacity;
            MaxCapacity = maxCapacity;
            DestoryTime = destoryTime;

            //创建缓存池
            _cache = new Queue<GameObject>();

            //加载资源
            _handle = ResourceManager.Instance.LoadAssetAsync<GameObject>(location);
            _handle.Completed += Handle_Completed;

        }

        private void Handle_Completed(AssetHandle obj)
        {
            //创建初始对象
            for (int i = 0; i < InitCapacity; i++)
            {
                GameObject cloneObj = InstantiateGameObject();//生成游戏对象
                SetRestoreCloneObject(cloneObj);//存储起来
                _cache.Enqueue(cloneObj);//进入对象池缓存
            }

            //最后返回结果
            for(int i=0;i<_loadingSpawn.Count;i++)
            {
                GameObject cloneObj = InstantiateGameObject();
                SpawnGameobject spawn = _loadingSpawn[i];
                spawn.Go = cloneObj;

                //注意Spawn当前的状态
                if(spawn.SpawnState==SpawnGameobject.ESpawnState.Restore)
                {
                    if(spawn.Go!=null)
                    {
                        RestoreGameObject(spawn.Go);
                    }
                }
                else if(spawn.SpawnState==SpawnGameobject.ESpawnState.Discard)
                {
                    if(spawn.Go!=null)
                    {
                        DiscardGameObject(spawn.Go);
                    }
                }
                else
                {
                    SetSpawnCloneObject(spawn.Go);//刷新物体位置，显示物体
                    spawn.UserCallback?.Invoke(spawn);
                }
            }
            _loadingSpawn.Clear();//清空物体
        }

        /// <summary>
        /// 加载游戏对象
        /// </summary>
        /// <returns></returns>
        private GameObject InstantiateGameObject()
        {
            var cloneObj = _handle.InstantiateSync();

            //如果加载失败，创建临时对象
            if (cloneObj == null)
                cloneObj = new GameObject(Location);

            return cloneObj;
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="go"></param>
        private void RestoreGameObject(GameObject go)
        {
            SetRestoreCloneObject(go);//先隐藏
            //判断对象池容量有没有超限
            if(_cache.Count<MaxCapacity)
            {
                //未超限，入池
                _cache.Enqueue(go);
            }
            else
            {
                //超限，直接销毁
                GameObject.Destroy(go);
            }
        }
        /// <summary>
        ///  销毁对象
        /// </summary>
        /// <param name="go"></param>
        private void DiscardGameObject(GameObject go)
        {
            GameObject.Destroy(go);
        }

        /// <summary>
        /// 隐藏物体
        /// </summary>
        /// <param name="cloneObj"></param>
        private void SetRestoreCloneObject(GameObject cloneObj)
        {
            cloneObj.SetActive(false);
            cloneObj.transform.SetParent(_root);
            cloneObj.transform.localPosition = Vector3.zero;
        }
        /// <summary>
        /// 显示物体
        /// </summary>
        /// <param name="cloneObj"></param>
        private void SetSpawnCloneObject(GameObject cloneObj)
        {
            cloneObj.SetActive(true);
            cloneObj.transform.SetParent(_root);
            cloneObj.transform.localPosition = Vector3.zero;
        }
        /// <summary>
        /// 查询静默时间内是否可以销毁
        /// </summary>
        public bool CanAutoDestory()
        {
            //持久性资源
            if (DontDestory)
                return false;
            //未设置销毁时间则不销毁
            if (DestoryTime < 0)
                return false;
            //上一次恢复保存的时间  资源的引用计数
            if (_lastRestoreRealTime > 0 && SpawnCount <= 0)
            {
                //判断当前时间与资源被最后一次恢复的时间之差是否超过了设置的可销毁时间DestroyTime
                //如果所有对象都回到了对象池，并且超出了DestoryTime，就认为这个对象池可以销毁了
                return (Time.realtimeSinceStartup - _lastRestoreRealTime) > DestoryTime;
            }
            else
                return false;
        }

        /// <summary>
        /// 获取游戏对象
        /// </summary>
        /// <param name="foceClone"></param>
        /// <param name="userDatas"></param>
        /// <returns></returns>
        public SpawnGameobject Spawn(bool foceClone,params System.Object[] userDatas)
        {
            SpawnGameobject spawn;

            //如果资源还没加载完毕
            if(isDone==false)
            {
                spawn = new SpawnGameobject(this, userDatas);
            }
            else
            {
                //是否强制生成
                if(foceClone==false&&_cache.Count>0)
                {
                    GameObject go = _cache.Dequeue();
                    spawn = new SpawnGameobject(this, go, userDatas);
                    SetSpawnCloneObject(spawn.Go);
                }
                else
                {
                    GameObject go = InstantiateGameObject();
                    spawn = new SpawnGameobject(this, go, userDatas);
                    SetSpawnCloneObject(go);
                }
            }
            SpawnCount++;
            return spawn;
        }

        /// <summary>
        /// 销毁对象池
        /// </summary>
        public void Destory()
        {
            //卸载资源对象
            _handle.Release();

            //销毁游戏对象
            foreach(var go in _cache)
            {
                if (go != null)
                    GameObject.Destroy(go);
            }
            _cache.Clear();

            //清空加载列表
            _loadingSpawn.Clear();
            SpawnCount = 0;
        }

        //回收游戏对象
        internal void Restore(SpawnGameobject spawn)
        {
            SpawnCount--;
            if(SpawnCount<=0)
            {
                _lastRestoreRealTime = Time.realtimeSinceStartup;
            }
            //资源有可能未加载完毕
            if (spawn.Go != null)
                RestoreGameObject(spawn.Go);
        }

        internal void Discard(SpawnGameobject spawn)
        {
            SpawnCount--;
            if(SpawnCount<=0)
            {
                _lastRestoreRealTime = Time.realtimeSinceStartup;
            }
            //资源有可能未加载完毕
            if (spawn.Go != null)
                DiscardGameObject(spawn.Go);
        }

        bool IEnumerator.MoveNext()
        {
            return !isDone;
        }

        public void Reset()
        {
            
        }
        object IEnumerator.Current
        {
            get { return null; }
        }
    }
}