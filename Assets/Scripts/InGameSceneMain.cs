using MLAPI;
using UnityEngine;

public class InGameSceneMain : BaseSceneMain
{
    public const int waitingPlayerCount = 2;

    private readonly GamePointAccumulator gamePointAccumulator = new GamePointAccumulator();
    private readonly PrefabCacheSystem enemyCacheSystem = new PrefabCacheSystem();
    private readonly PrefabCacheSystem bulletCacheSystem = new PrefabCacheSystem();
    private readonly PrefabCacheSystem effectCacheSystem = new PrefabCacheSystem();
    private readonly PrefabCacheSystem damageCacheSystem = new PrefabCacheSystem();

    private int playerCount = 1;

    [SerializeField] private Player player = null;
    [SerializeField] private Transform mainBGQuadTransform = null;
    [SerializeField] private EffectManager effectManager = null;
    [SerializeField] private BulletManager bulletManager = null;
    [SerializeField] private EnemyManager enemyManager = null;
    [SerializeField] private DamageManager damageManager = null;
    [SerializeField] private SquadronManager squadronManager = null;
    [SerializeField] private InGameNetworkTransfer inGameNetworkTransfer = null;
    [SerializeField] private Transform playerStartTransform1 = null;
    [SerializeField] private Transform playerStartTransform2 = null;

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
        set => player = value;
    }

    public Transform MainBGQuadTransform => mainBGQuadTransform;
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
    public InGameNetworkTransfer InGameNetworkTransfer => inGameNetworkTransfer;
    public Transform PlayerStartTransform1 => playerStartTransform1;
    public Transform PlayerStartTransform2 => playerStartTransform2;
    public GameState CurrentGameState => inGameNetworkTransfer.CurrentGameState;

    public void GameStart()
    {
        inGameNetworkTransfer.GameStartClientRpc();
    }

    protected override void OnStart()
    {
        base.OnStart();
        Debug.Log("NetworkManager: " + NetworkManager.Singleton);
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
    }

    private void OnClientConnectedCallback(ulong obj)
    {
        Debug.Log("OnClientConnectedCallback : " + obj);

        playerCount++;

        if (playerCount >= waitingPlayerCount)
        {
            GameStart();
        }
    }
}
