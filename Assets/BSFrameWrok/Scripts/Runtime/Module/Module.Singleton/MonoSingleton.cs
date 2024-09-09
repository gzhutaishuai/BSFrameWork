using UnityEngine;

public class MonoSingleton<T> :MonoBehaviour where T:MonoSingleton<T>
{
    protected MonoSingleton() { }

    private static T _instance;

    public static bool isExisted { get; private set; }=false;
    public static T Instance
    {
        get
        {
            if(_instance == null)
                _instance=FindObjectOfType<T>();
            if(_instance == null)
            {
                GameObject go = new GameObject(typeof(T).Name);
                
                _instance = go.AddComponent<T>();

                isExisted = true;
            }
            return _instance;   
        }
    }

    public virtual void OnDestory()
    {
        isExisted=false;
    }
}
