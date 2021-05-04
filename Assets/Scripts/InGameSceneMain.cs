using UnityEngine;

public class InGameSceneMain : BaseSceneMain
{
    public enum GameState : int
    {
        Ready = 0,
        Running,
        End
    }

    private const float gameReadyInterval = 3.0f;

    private readonly GamePointAccumulator gamePointAccumulator = new GamePointAccumulator();
    private readonly PrefabCacheSystem enemyCacheSystem = new PrefabCacheSystem();
    private readonly PrefabCacheSystem bulletCacheSystem = new PrefabCacheSystem();
    private readonly PrefabCacheSystem effectCacheSystem = new PrefabCacheSystem();
    private readonly PrefabCacheSystem damageCacheSystem = new PrefabCacheSystem();

    private GameState currentGameState = GameState.Ready;
    private float sceneStartTime;

    [SerializeField] private Player player = null;
    [SerializeField] private EffectManager effectManager = null;
    [SerializeField] private BulletManager bulletManager = null;
    [SerializeField] private EnemyManager enemyManager = null;
    [SerializeField] private DamageManager damageManager = null;
    [SerializeField] private SquadronManager squadronManager = null;

    public GameState CurrentGameState => currentGameState;

    public Player Player
    {
        get
        {
            if (!player)
            {
                Debug.LogError("Main Player is not setted!");
            }

            return player;
        }
    }

    public GamePointAccumulator GamePointAccumulator => gamePointAccumulator;
    public PrefabCacheSystem EnemyCacheSystem => enemyCacheSystem;
    public PrefabCacheSystem BulletCacheSystem => bulletCacheSystem;
    public PrefabCacheSystem EffectCacheSystem => effectCacheSystem;
    public PrefabCacheSystem DamageCacheSystem => damageCacheSystem;
    public EffectManager EffectManager => effectManager;
    public BulletManager BulletManager => bulletManager;
    public EnemyManager EnemyManager => enemyManager;
    public DamageManager DamageManager => damageManager;
    public SquadronManager SquadronManager => squadronManager;

    protected override void OnStart()
    {
        base.OnStart();
        sceneStartTime = Time.time;
    }

    protected override void UpdateScene()
    {
        base.UpdateScene();

        float currentTime = Time.time;
        if (currentGameState == GameState.Ready)
        {
            if (currentTime - sceneStartTime > gameReadyInterval)
            {
                SquadronManager.StartGame();
                currentGameState = GameState.Running;
            }
        }
    }
}
