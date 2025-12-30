using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField]
    private bool dontDestroyOnLoad = false;
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>(FindObjectsInactive.Include);
                if(_instance == null)
                    Debug.LogWarning($"[SingletonBehaviour] There is no instance of {typeof(T).Name} in the scene.");
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        /*if (_instance != null)
        {
            Destroy(this);
            return;
        }*/

        _instance = GetComponent<T>();
        if(dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);
    }
}