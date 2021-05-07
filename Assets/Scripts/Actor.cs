// #define NETWORK_BEHAVIOUR // 정상적으로 NetworkBehaviour 인스턴스의 Update로 호출되어 실행되고 있을 때
#define MONO_BEHAVIOUR // MohoBehaviour 인스턴스의 Update로 호출되어 실행되고 있을 때
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

public class Actor : NetworkBehaviour
{
    [SerializeField] protected int maxHp = 100;
    [SerializeField] protected int currentHp;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected int crashDamage = 100;
    [SerializeField] private bool isDead = false;

    public bool IsDead => isDead;
    protected int CrashDamage => crashDamage;

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
#if NETWORK_BEHAVIOUR
        SetPositionServerRpc(position);
#elif MONO_BEHAVIOUR
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
#endif
    }

    [ClientRpc]
    public void SetActiveClientRpc(bool value)
    {
        gameObject.SetActive(value);
    }

    protected virtual void Initialize()
    {
        currentHp = maxHp;
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

        currentHp -= value;

        if (currentHp < 0)
        {
            currentHp = 0;
        }

        if (currentHp == 0)
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

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        UpdateActor();
    }
}
