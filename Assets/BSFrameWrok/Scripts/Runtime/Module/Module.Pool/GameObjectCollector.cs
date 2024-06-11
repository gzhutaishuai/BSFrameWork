using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace GameFW.Pool
{
    public class GameObjectCollector : IEnumerator
    {
        private readonly Queue<GameObject> _cache;//�Ѿ����ؽ��ڴ浫δ��ʹ�õ���Ϸ����ʵ��
        private readonly List<SpawnGameobject> _loadingSpawn = new List<SpawnGameobject>();//���ڼ��ص���δ������ɵ���Ϸ����ʵ��
        private readonly Transform _root;//�����Ѿ����ز����գ���δʹ�õ���Ϸ����ĸ��ڵ�
        private AssetHandle _handle;//��Դ�����������ȡ��Դ���صĹ��̺ͽ��
        private float _lastRestoreRealTime = -1f;//���ж��󱻹黹������ص�ʱ��

        /// <summary>
        /// ��Դ��λ��ַ
        /// </summary>
        public string Location { private set; get; }

        /// <summary>
        /// ��Դ��פ������
        /// </summary>
        public bool DontDestory { private set; get; }

        /// <summary>
        /// ����صĳ�ʼ����
        /// </summary>
        public int InitCapacity { private set; get; }

        /// <summary>
        /// ����ص��������
        /// </summary>
        public int MaxCapacity { private set; get; }

        /// <summary>
        /// ��Ĭ����ʱ��
        /// </summary>
        public float DestoryTime { private set; get; }

        /// <summary>
        /// �Ƿ�������
        /// </summary>
        public bool isDone
        {
            get
            {
                return _handle.IsDone;
            }
        }

        /// <summary>
        /// ��ǰ����״̬
        /// </summary>
        public EOperationStatus States
        {
            get
            {
                return _handle.Status;
            }
        }

        /// <summary>
        /// �ڲ���������
        /// </summary>
        public int CacheCount
        {
            get
            {
               return _cache.Count;
            }
        }

        /// <summary>
        /// �ⲿʹ������
        /// </summary>
        public int SpawnCount { private set; get; } = 0;

        /// <summary>
        /// ���캯��
        /// </summary>
        public GameObjectCollector(Transform root,string location,bool dontDestory,int initCapacity,int maxCapacity,float destoryTime)
        {
            _root = root;
            Location = location;
            DontDestory = dontDestory;
            InitCapacity = initCapacity;
            MaxCapacity = maxCapacity;
            DestoryTime = destoryTime;

            //���������
            _cache = new Queue<GameObject>();

            //������Դ
            _handle = ResourceManager.Instance.LoadAssetAsync<GameObject>(location);
            _handle.Completed += Handle_Completed;

        }

        private void Handle_Completed(AssetHandle obj)
        {
            //������ʼ����
            for (int i = 0; i < InitCapacity; i++)
            {
                GameObject cloneObj = InstantiateGameObject();//������Ϸ����
                SetRestoreCloneObject(cloneObj);//�洢����
                _cache.Enqueue(cloneObj);//�������ػ���
            }

            //��󷵻ؽ��
            for(int i=0;i<_loadingSpawn.Count;i++)
            {
                GameObject cloneObj = InstantiateGameObject();
                SpawnGameobject spawn = _loadingSpawn[i];
                spawn.Go = cloneObj;

                //ע��Spawn��ǰ��״̬
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
                    SetSpawnCloneObject(spawn.Go);//ˢ������λ�ã���ʾ����
                    spawn.UserCallback?.Invoke(spawn);
                }
            }
            _loadingSpawn.Clear();//�������
        }

        /// <summary>
        /// ������Ϸ����
        /// </summary>
        /// <returns></returns>
        private GameObject InstantiateGameObject()
        {
            var cloneObj = _handle.InstantiateSync();

            //�������ʧ�ܣ�������ʱ����
            if (cloneObj == null)
                cloneObj = new GameObject(Location);

            return cloneObj;
        }

        /// <summary>
        /// ���ն���
        /// </summary>
        /// <param name="go"></param>
        private void RestoreGameObject(GameObject go)
        {
            SetRestoreCloneObject(go);//������
            //�ж϶����������û�г���
            if(_cache.Count<MaxCapacity)
            {
                //δ���ޣ����
                _cache.Enqueue(go);
            }
            else
            {
                //���ޣ�ֱ������
                GameObject.Destroy(go);
            }
        }
        /// <summary>
        ///  ���ٶ���
        /// </summary>
        /// <param name="go"></param>
        private void DiscardGameObject(GameObject go)
        {
            GameObject.Destroy(go);
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="cloneObj"></param>
        private void SetRestoreCloneObject(GameObject cloneObj)
        {
            cloneObj.SetActive(false);
            cloneObj.transform.SetParent(_root);
            cloneObj.transform.localPosition = Vector3.zero;
        }
        /// <summary>
        /// ��ʾ����
        /// </summary>
        /// <param name="cloneObj"></param>
        private void SetSpawnCloneObject(GameObject cloneObj)
        {
            cloneObj.SetActive(true);
            cloneObj.transform.SetParent(_root);
            cloneObj.transform.localPosition = Vector3.zero;
        }
        /// <summary>
        /// ��ѯ��Ĭʱ�����Ƿ��������
        /// </summary>
        public bool CanAutoDestory()
        {
            //�־�����Դ
            if (DontDestory)
                return false;
            //δ��������ʱ��������
            if (DestoryTime < 0)
                return false;
            //��һ�λָ������ʱ��  ��Դ�����ü���
            if (_lastRestoreRealTime > 0 && SpawnCount <= 0)
            {
                //�жϵ�ǰʱ������Դ�����һ�λָ���ʱ��֮���Ƿ񳬹������õĿ�����ʱ��DestroyTime
                //������ж��󶼻ص��˶���أ����ҳ�����DestoryTime������Ϊ�������ؿ���������
                return (Time.realtimeSinceStartup - _lastRestoreRealTime) > DestoryTime;
            }
            else
                return false;
        }

        /// <summary>
        /// ��ȡ��Ϸ����
        /// </summary>
        /// <param name="foceClone"></param>
        /// <param name="userDatas"></param>
        /// <returns></returns>
        public SpawnGameobject Spawn(bool foceClone,params System.Object[] userDatas)
        {
            SpawnGameobject spawn;

            //�����Դ��û�������
            if(isDone==false)
            {
                spawn = new SpawnGameobject(this, userDatas);
            }
            else
            {
                //�Ƿ�ǿ������
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
        /// ���ٶ����
        /// </summary>
        public void Destory()
        {
            //ж����Դ����
            _handle.Release();

            //������Ϸ����
            foreach(var go in _cache)
            {
                if (go != null)
                    GameObject.Destroy(go);
            }
            _cache.Clear();

            //��ռ����б�
            _loadingSpawn.Clear();
            SpawnCount = 0;
        }

        //������Ϸ����
        internal void Restore(SpawnGameobject spawn)
        {
            SpawnCount--;
            if(SpawnCount<=0)
            {
                _lastRestoreRealTime = Time.realtimeSinceStartup;
            }
            //��Դ�п���δ�������
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
            //��Դ�п���δ�������
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