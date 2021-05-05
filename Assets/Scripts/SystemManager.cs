using UnityEngine;

public class SystemManager : MonoBehaviour
{
    private static SystemManager instance = null;
    public static SystemManager Instance => instance;

    private BaseSceneMain currentSceneMain;

    [SerializeField] private EnemyTable enemyTable = null;

    public BaseSceneMain CurrentSceneMain
    {
        get => currentSceneMain;
        set
        {
            currentSceneMain = value;
            if (value != null)
            {
                CurrentSceneMainChanged?.Invoke(this, value.name);
            }
        }
    }
    public EnemyTable EnemyTable => enemyTable;

    public event System.EventHandler<string> CurrentSceneMainChanged;

    public T GetCurrentSceneMain<T>() where T : BaseSceneMain
    {
        return CurrentSceneMain as T;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("SystemManager is initialized twice!");
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Scene 이동간에 사라지지 않도록 처리
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        BaseSceneMain baseSceneMain = FindObjectOfType<BaseSceneMain>();
        Debug.Log("OnSceneLoaded! baseSceneMain.name = " + baseSceneMain.name);
        Instance.CurrentSceneMain = baseSceneMain;
    }
}
