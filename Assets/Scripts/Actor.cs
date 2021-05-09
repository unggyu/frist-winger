using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

public class Actor : NetworkBehaviour
{
    /// <summary>
    /// Actor Id
    /// </summary>
    protected readonly NetworkVariable<int> actorInstanceId =
        new NetworkVariable<int>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.ServerOnly }, 0);

    /// <summary>
    /// 최대 체력
    /// </summary>
    [SerializeField] protected readonly NetworkVariable<int> maxHp = 
        new NetworkVariable<int>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, 100);

    /// <summary>
    /// 현재 체력
    /// </summary>
    [SerializeField] protected readonly NetworkVariable<int> currentHp =
        new NetworkVariable<int>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });

    /// <summary>
    /// 데미지
    /// </summary>
    [SerializeField] protected readonly NetworkVariable<int> damage =
        new NetworkVariable<int>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, 1);

    /// <summary>
    /// 충돌 데미지
    /// </summary>
    [SerializeField] protected readonly NetworkVariable<int> crashDamage =
        new NetworkVariable<int>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, 100);

    /// <summary>
    /// 죽었는지 여부
    /// </summary>
    [SerializeField] private bool isDead = false;

    /// <summary>
    /// Actor Id
    /// </summary>
    public int ActorInstanceId => actorInstanceId.Value;

    /// <summary>
    /// 죽었는지 여부
    /// </summary>
    public bool IsDead => isDead;

    /// <summary>
    /// 충돌 데미지
    /// </summary>
    protected int CrashDamage => crashDamage.Value;

    public virtual void OnBulletHited(Actor attacker, int damage, Vector3 hitPos)
    {
        Debug.Log("OnBulletHited damage = " + damage);
        DecreaseHp(attacker, damage, hitPos);
    }

    public virtual void OnCrash(Actor attacker, int damage, Vector3 crashPos)
    {
        Debug.Log("OnCrash attacker = " + attacker.name + ", damage = " + damage);
        DecreaseHp(attacker, damage, crashPos);
    }

    public void SetPosition(Vector3 position)
    {
        if (IsServer)
        {
            SetPositionClientRpc(position);
        }
        else
        {
            SetPositionServerRpc(position);
            if (IsLocalPlayer)
            {
                transform.position = position;
            }
        }
    }

    [ClientRpc]
    public void SetActiveClientRpc(bool value)
    {
        gameObject.SetActive(value);
    }

    protected virtual void Initialize()
    {
        currentHp.Value = maxHp.Value;

        if (IsServer)
        {
            actorInstanceId.Value = GetInstanceID();
        }
    }

    protected virtual void UpdateActor()
    {
    }

    protected virtual void DecreaseHp(Actor attacker, int value, Vector3 damagePos)
    {
        if (isDead)
        {
            return;
        }

        currentHp.Value -= value;

        if (currentHp.Value < 0)
        {
            currentHp.Value = 0;
        }

        if (currentHp.Value == 0)
        {
            OnDead(attacker);
        }
    }

    protected virtual void OnDead(Actor killer)
    {
        Debug.Log(name + " OnDead");
        isDead = true;

        SystemManager
            .Instance
            .GetCurrentSceneMain<InGameSceneMain>()
            .EffectManager
            .GenerateEffect(EffectManager.ActorDeadFxIndex, transform.position);
    }

    private void Awake()
    {
        actorInstanceId.OnValueChanged += OnActorInstanceIdChanged;
    }

    private void Start()
    {
        if (SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>() != null)
        {
            Initialize();
        }
        else
        {
            SystemManager.Instance.CurrentSceneMainChanged += CurrentSceneMainChanged;
        }
    }

    private void Update()
    {
        UpdateActor();
    }

    [ServerRpc]
    private void SetPositionServerRpc(Vector3 position)
    {
        transform.position = position;
    }

    [ClientRpc]
    private void SetPositionClientRpc(Vector3 position)
    {
        transform.position = position;
    }

    private void OnActorInstanceIdChanged(int previousId, int newId)
    {
        if (IsServer)
        {
            return;
        }

        if (actorInstanceId.Value != 0)
        {
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ActorManager.Regist(actorInstanceId.Value, this);
        }
    }

    private void CurrentSceneMainChanged(object sender, string sceneName)
    {
        if (sceneName.Equals(nameof(InGameSceneMain)))
        {
            Initialize();
        }

        SystemManager.Instance.CurrentSceneMainChanged -= CurrentSceneMainChanged;
    }
}
