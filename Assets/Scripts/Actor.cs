using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

public class Actor : NetworkBehaviour
{
    protected readonly NetworkVariable<bool> isActive =
        new NetworkVariable<bool>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.ServerOnly }, true);

    /// <summary>
    /// Actor Id
    /// </summary>
    protected readonly NetworkVariable<int> actorInstanceId =
        new NetworkVariable<int>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.ServerOnly }, 0);

    /// <summary>
    /// 최대 체력
    /// </summary>
    [SerializeField]
    protected readonly NetworkVariable<int> maxHp = 
        new NetworkVariable<int>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, 100);

    /// <summary>
    /// 현재 체력
    /// </summary>
    [SerializeField]
    protected readonly NetworkVariable<int> currentHp =
        new NetworkVariable<int>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });

    /// <summary>
    /// 데미지
    /// </summary>
    [SerializeField]
    protected readonly NetworkVariable<int> damage =
        new NetworkVariable<int>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, 100);

    /// <summary>
    /// 충돌 데미지
    /// </summary>
    [SerializeField]
    protected readonly NetworkVariable<int> crashDamage =
        new NetworkVariable<int>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, 100);

    /// <summary>
    /// 죽었는지 여부
    /// </summary>
    [SerializeField] protected bool isDead = false;

    /// <summary>
    /// 초기화 여부
    /// </summary>
    private bool initialized = false;

    /// <summary>
    /// Actor 등록이 필요한지 여부
    /// </summary>
    private bool isNeedRegistActor = false;

    /// <summary>
    /// Actor Id
    /// </summary>
    public int ActorInstanceId => actorInstanceId.Value;

    /// <summary>
    /// 최대 체력
    /// </summary>
    public int MaxHp => maxHp.Value;

    /// <summary>
    /// 현재 체력
    /// </summary>
    public int CurrentHp => currentHp.Value;

    /// <summary>
    /// 죽었는지 여부
    /// </summary>
    public bool IsDead => isDead;

    public bool IsActive
    {
        get => isActive.Value;
        set
        {
            if (IsServer)
            {
                // true는 ClientRpc로만 설정이 가능하고
                // false는 NetworkVariable으로만 설정이 가능함
                // 이유는 정확히 모르겠지만 MLAPI 자체 문제로 생각됨
                isActive.Value = value;
                if (value)
                {
                    SetActiveClientRpc(value);
                }
            }
        }
    }

    /// <summary>
    /// 충돌 데미지
    /// </summary>
    protected int CrashDamage => crashDamage.Value;

    public virtual void OnBulletHited(int damage, Vector3 hitPos)
    {
        Debug.Log("OnBulletHited damage = " + damage);
        DecreaseHp(damage, hitPos);
    }

    public virtual void OnCrash(int damage, Vector3 crashPos)
    {
        DecreaseHp(damage, crashPos);
    }

    public void SetPosition(Vector3 position)
    {
        if (IsServer)
        {
            SetPositionClientRpc(position);
        }
        else if (!IsServer && IsOwner)
        {
            SetPositionServerRpc(position);
            if (IsLocalPlayer)
            {
                transform.position = position;
            }
        }
    }

    protected virtual void Initialize()
    {
        isActive.OnValueChanged += OnIsActiveChanged;
        actorInstanceId.OnValueChanged += OnActorInstanceIdChanged;

        currentHp.Value = maxHp.Value;

        if (IsServer)
        {
            actorInstanceId.Value = GetInstanceID();
        }

        initialized = true;
    }

    protected virtual void UpdateActor()
    {
    }

    protected virtual void DecreaseHp(int value, Vector3 damagePos)
    {
        if (IsDead)
        {
            return;
        }

        if (IsServer)
        {
            DecreaseHpClientRpc(value, damagePos);
        }
        else if (!IsServer && IsOwner)
        {
            DecreaseHpServerRpc(value, damagePos);
            if (IsLocalPlayer)
            {
                DecreaseHpInternal(value, damagePos);
            }
        }
    }

    protected virtual void DecreaseHpInternal(int value, Vector3 damagePos)
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
            OnDead();
        }
    }

    protected virtual void OnDead()
    {
        Debug.Log(name + " OnDead");
        isDead = true;

        SystemManager
            .Instance
            .GetCurrentSceneMain<InGameSceneMain>()
            .EffectManager
            .GenerateEffect(EffectManager.ActorDeadFxIndex, transform.position);
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

    private void OnDestroy()
    {
        isActive.OnValueChanged -= OnIsActiveChanged;
        actorInstanceId.OnValueChanged -= OnActorInstanceIdChanged;
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

    [ServerRpc]
    private void DecreaseHpServerRpc(int value, Vector3 damagePos)
    {
        DecreaseHpInternal(value, damagePos);
    }

    [ClientRpc]
    private void DecreaseHpClientRpc(int value, Vector3 damagePos)
    {
        DecreaseHpInternal(value, damagePos);
    }

    [ClientRpc]
    private void SetActiveClientRpc(bool value)
    {
        gameObject.SetActive(value);
    }

    private void RegistActor()
    {
        if (actorInstanceId.Value != 0)
        {
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ActorManager.Regist(actorInstanceId.Value, this);
        }
    }

    private void OnActorInstanceIdChanged(int previousId, int newId)
    {
        if (IsServer)
        {
            return;
        }

        if (SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>() != null)
        {
            RegistActor();
        }
        else
        {
            isNeedRegistActor = true;
            SystemManager.Instance.CurrentSceneMainChanged += CurrentSceneMainChanged;
        }
    }

    private void OnIsActiveChanged(bool previousValue, bool newValue)
    {
        if (gameObject.activeSelf != newValue)
        {
            gameObject.SetActive(newValue);
        }
    }

    private void CurrentSceneMainChanged(object sender, string sceneName)
    {
        if (sceneName.Equals(nameof(InGameSceneMain)))
        {
            if (!initialized)
            {
                Initialize();
            }

            if (isNeedRegistActor)
            {
                RegistActor();
                isNeedRegistActor = false;
            }
        }

        SystemManager.Instance.CurrentSceneMainChanged -= CurrentSceneMainChanged;
    }
}
