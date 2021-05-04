using UnityEngine;

public class SystemManager : MonoBehaviour
{
    private static SystemManager instance = null;
    public static SystemManager Instance => instance;

    [SerializeField] private Player player = null;
    [SerializeField] private EffectManager effectManager = null;
    [SerializeField] private BulletManager bulletManager = null;
    [SerializeField] private EnemyManager enemyManager = null;
    [SerializeField] private DamageManager damageManager = null;
    [SerializeField] private EnemyTable enemyTable = null;

    private readonly GamePointAccumulator gamePointAccumulator = new GamePointAccumulator();
    private readonly PrefabCacheSystem enemyCacheSystem = new PrefabCacheSystem();
    private readonly PrefabCacheSystem bulletCacheSystem = new PrefabCacheSystem();
    private readonly PrefabCacheSystem effectCacheSystem = new PrefabCacheSystem();
    private readonly PrefabCacheSystem damageCacheSystem = new PrefabCacheSystem();

    public Player Player => player;
    public EffectManager EffectManager => effectManager;
    public BulletManager BulletManager => bulletManager;
    public EnemyManager EnemyManager => enemyManager;
    public DamageManager DamageManager => damageManager;
    public EnemyTable EnemyTable => enemyTable;
    public GamePointAccumulator GamePointAccumulator => gamePointAccumulator;
    public PrefabCacheSystem EnemyCacheSystem => enemyCacheSystem;
    public PrefabCacheSystem BulletCacheSystem => bulletCacheSystem;
    public PrefabCacheSystem EffectCacheSystem => effectCacheSystem;
    public PrefabCacheSystem DamageCacheSystem => damageCacheSystem;

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
}
