using UnityEngine;

public class Actor : MonoBehaviour
{
    [SerializeField]
    protected int maxHp = 100;

    [SerializeField]
    protected int currentHp;

    [SerializeField]
    protected int damage = 1;

    [SerializeField]
    protected int crashDamage = 100;

    [SerializeField]
    bool isDead = false;

    public bool IsDead
    {
        get => isDead;
    }

    protected int CrashDamage
    {
        get => crashDamage;
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        currentHp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateActor();
    }

    protected virtual void UpdateActor()
    {

    }

    public virtual void OnBulletHited(Actor attacker, int damage)
    {
        Debug.Log("OnBulletHited damage = " + damage);
        DecreaseHp(attacker, damage);
    }

    public virtual void OnCrash(Actor attacker, int damage)
    {
        Debug.Log("OnCrash attacker = " + attacker.name + ", damage = " + damage);
        DecreaseHp(attacker, damage);
    }

    void DecreaseHp(Actor attacker, int value)
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
    }
}
