using UnityEngine;

public class SystemManager : MonoBehaviour
{
    static SystemManager instance = null;

    public static SystemManager Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField]
    Player player = null;

    GamePointAccumulator gamePointAccumulator = new GamePointAccumulator();

    [SerializeField]
    EffectManager effectManager;

    public Player Player
    {
        get => player;
    }

    public GamePointAccumulator GamePointAccumulator
    {
        get => gamePointAccumulator;
    }

    public EffectManager EffectManager
    {
        get => effectManager;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
