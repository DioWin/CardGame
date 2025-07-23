using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected Singleton() { }
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType<T>();
            if (instance == null)
            {
                Object manager = Resources.Load(typeof(T).Name);
                if (manager != null)
                    Instantiate(manager);
            }
            return instance;
        }
    }

}
