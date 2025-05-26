using UnityEngine;

public class SingleTon<T> : MonoBehaviour where T : MonoBehaviour 
{
    private static T instance;
    public static T Instance
    {
        get
        {
            instance = (T)FindAnyObjectByType(typeof(T));
            if(instance == null)
            {
                GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                instance = obj.GetComponent<T>();
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        // 중복 방지 (DontDestroyOnLoad 안 쓰므로 씬마다 생겨도 됨)
        if (instance == null)
        {
            instance = this as T;
        }
        else if (instance != this)
        {
            Debug.LogWarning($"{typeof(T).Name} 중복 인스턴스 제거");
            Destroy(gameObject); // 중복된 거 파괴
        }
    }
}