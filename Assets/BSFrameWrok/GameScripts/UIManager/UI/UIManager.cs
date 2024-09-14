using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;    
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.UI
{
    /// <summary>UI管理器/// </summary>
    public class UIManager : MonoBehaviour
    {
        /// <summary>UI面板的集合，记录当前场景中的所有面板// </summary>
        public static Dictionary<Type, UIPanelBase> PanelDic = null;

        /// <summary>存放面板层级的集合// </summary>
        public static Dictionary<UIPanelLayer, RectTransform> layers = null;


        #region 初始化各种组件
        /// <summary>UI画布// </summary>
        public static Canvas Canvas { set; private get; } = null;

        /// <summary>UI事件响应// </summary>
        public static EventSystem EventSystem { set; private get; } = null;

        /// <summary>UIManager单例，全局唯一// </summary>
        public static UIManager Instance { set; private get; } = null;

        /// <summary>UI正交相机// </summary>
        public static Camera Camera { set; private get; } = null;
        #endregion

        /// <summary>获取指定的UI层级位置// </summary>
        public static RectTransform GetLayer(UIPanelLayer layer)
        {
            return layers[layer];
        }

        /// <summary>
        /// 初始化UI框架
        /// </summary>
        public static void Init()
        {
            PanelDic = new Dictionary<Type, UIPanelBase>();

            //克隆UIManager
            var obj = Resources.Load("Prefabs/UI/UIRoot/UIRoot");
            Instance = GameObject.Instantiate(obj).AddComponent<UIManager>();
            Instance.name = nameof(UIManager);
            DontDestroyOnLoad(Instance);

            //获取组件
            Canvas = Instance.GetComponentInChildren<Canvas>();
            EventSystem = Instance.GetComponentInChildren<EventSystem>();
            //Camera = Instance.GetComponentInChildren<Camera>();

            //获取UIRoot的所有层级
            layers = new Dictionary<UIPanelLayer, RectTransform>();

            //从枚举中赋值
            foreach (UIPanelLayer layer in Enum.GetValues(typeof(UIPanelLayer)))
            {
                //从画布的子物体下找到对应名称的UI层级，通过转换，变为RectTransfrom
                layers.Add(layer, Canvas.transform.Find(layer.ToString()) as RectTransform);
            }
            Debug.Log("UIRoot建立成功");
        }


        /// <summary>
        /// 打开一个面板
        /// </summary>
        /// <typeparam name="T">要打开的面板类型</typeparam>
        /// <param name="data">要传入的数据</param>
        public static void Open<T>(object data = null) where T : UIPanelBase
        {
            //打开面板
            void openPanel(UIPanelBase panel)
            {
                //将新打开的UI面板设置为当前层级的最下方，最后渲染，最先显示
                panel.transform.SetAsLastSibling();

                //传入数据
                panel.SetData(data);

                //打开面板
                panel.OnUIEnable();
            }

            UIPanelBase clonePanel()
            {
                //查找面板路径
                var panelRootPath = "Prefabs/UI";
                var panelSubPath = $"UIPanel/{typeof(T).Name}";
                var panelPath = $"{panelRootPath}/{panelSubPath}";

                //加载面板
                var obj = Resources.Load(panelPath);
                if (obj == null)
                {
                    Debug.LogError($"该路径下:{panelRootPath}未查找到此面板:{typeof(T).Name}");
                }

                //查找面板所在层级
                RectTransform layer = UIManager.GetLayer(UIPanelLayer.Game);

                var objects = typeof(T).GetCustomAttributes(typeof(UILayerAttribute), true);
                if (objects?.Length > 0)
                {
                    var layerAttr = objects[0] as UILayerAttribute;
                    layer = UIManager.GetLayer(layerAttr.layer);
                }

                //克隆面板
                var go = GameObject.Instantiate(obj, layer);
                go.name = typeof(T).Name;

                var newPanel = go.AddComponent<T>();//增加面板对应的脚本

                return newPanel;

            }

            if (PanelDic.TryGetValue(typeof(T), out UIPanelBase panel))
            {

                //打开面板
                openPanel(panel);
            }
            else
            {
                //克隆面板
                panel = clonePanel();

                //将新面板记录到字典中去
                PanelDic.Add(typeof(T), panel);

                //初始化面板
                panel.OnUIAwake();

                //延迟一帧调用OnUIStart
                Instance.StartCoroutine(Invoke(panel.OnUIStart));

                //打开面板
                openPanel(panel);
            }
        }

        /// <summary>
        /// 关闭一个面板
        /// </summary>
        /// <typeparam name="T">要关闭的面板类型</typeparam>
        public static void Close<T>() where T : UIPanelBase
        {
            if (!PanelDic.TryGetValue(typeof(T), out UIPanelBase panel))
            {
                Debug.LogError($"场景中未查找到{typeof(T).Name}面板");
            }
            else
            {
                Close(panel);
            }
        }
        /// <summary>
        /// 关闭面板的方法扩展
        /// </summary>
        /// <param name="panel">参数不同于T，可以直接传入面板类型</param>
        public static void Close(UIPanelBase panel)
        {
            panel?.OnUIDisable();
        }
        /// <summary>
        /// 关闭所有面板
        /// </summary>
        public static void CloseAll()
        {
            foreach (var panel in PanelDic.Values)
            {
                UIManager.Close(panel);
            }
        }
        /// <summary>
        /// 删除一个面板
        /// </summary>
        /// <typeparam name="T">要删除的面板类型</typeparam>
        public static void Destory<T>() where T : UIPanelBase
        {
            if (!PanelDic.TryGetValue(typeof(T), out UIPanelBase panel))
            {
                Debug.LogWarning($"场景中未查找到{typeof(T).Name}面板");
            }
            else
            {
                UIManager.Destory(panel);
            }
        }
        /// <summary>
        /// 删除面板的扩展方法
        /// </summary>
        /// <param name="panel">参数不同，可以直接传入面板类型</param>
        public static void Destory(UIPanelBase panel)
        {
            panel?.OnUIDisable();//先关闭panel
            panel?.OnUIDestory();

            //清除脏数据
            if (PanelDic.ContainsKey(panel.GetType()))
            {
                PanelDic.Remove(panel.GetType());
            }
        }
        /// <summary>
        /// 删除所有面板
        /// </summary>
        public static void DestoryAll()
        {
            List<UIPanelBase> panels = new List<UIPanelBase>(PanelDic.Values);
            foreach (var panel in panels)
            {
                UIManager.Destory(panel);
            }
            panels.Clear();
            PanelDic.Clear();
        }
        /// <summary>
        /// 获取一个面板
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>() where T : UIPanelBase
        {
            if (PanelDic.TryGetValue(typeof(T), out UIPanelBase panel))
            {
                return panel as T;
            }
            else
            {
                Debug.LogError($"场景中未查找到{typeof(T).Name}面板");
                return default(T);
            }

        }
        /// <summary>
        /// 保证OnUIStart后于OnUIAwake调用
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        private static IEnumerator Invoke(Action callback)
        {
            yield return new WaitForEndOfFrame();

            callback?.Invoke();
        }
    }
}
