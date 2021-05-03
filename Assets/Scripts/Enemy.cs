using UnityEngine;

public class Enemy : Actor
{
    private enum State : int
    {
        /// <summary>
        /// 사용전
        /// </summary>
        None = -1,

        /// <summary>
        /// 준비완료
        /// </summary>
        Ready = 0,

        /// <summary>
        /// 등장
        /// </summary>
        Appear,

        /// <summary>
        /// 전투중
        /// </summary>
        Battle,

        /// <summary>
        /// 사망
        /// </summary>
        Dead,

        /// <summary>
        /// 퇴장
        /// </summary>
        Disappear
    }

    private const float maxSpeed = 10.0f;
    private const float maxSpeedTime = 0.5f;

    private float lastActionUpdateTime = 0.0f;
    private float moveStartTime = 0.0f;
    private Vector3 currentVelocity = Vector3.zero;

    /// <summary>
    /// 입장 시 도착 위치
    /// </summary>
    private Vector3 appearPoint = Vector3.zero;

    /// <summary>
    /// 퇴장 시 목표 위치
    /// </summary>
    private Vector3 disappearPoint = Vector3.zero;

    [SerializeField] private State currentState = State.None;
    [SerializeField] private Vector3 targetPosition = Vector3.zero;
    [SerializeField] private float currentSpeed = 0.0f;
    [SerializeField] private Transform fireTransform = null;
    [SerializeField] private float bulletSpeed = 1.0f;
    [SerializeField] private int fireRemainCount = 1;
    [SerializeField] private int gamePoint = 0;

    public string FilePath { get; set; }

    public void Appear(Vector3 targetPos)
    {
        targetPosition = targetPos;
        currentSpeed = maxSpeed;

        currentState = State.Appear;
        moveStartTime = Time.time;
    }

    public void Fire()
    {
        Bullet bullet = SystemManager.Instance.BulletManager.Generate(BulletManager.EnemyBulletIndex);
        bullet.Fire(this, fireTransform.position, -fireTransform.right, bulletSpeed, damage);
    }

    public void Reset(EnemyGenerateData data)
    {
        currentHp = maxHp = data.maxHp;
        damage = data.damage;
        crashDamage = data.crashDamage;
        bulletSpeed = data.bulletSpeed;
        fireRemainCount = data.fireRemainCount;
        gamePoint = data.gamePoint;

        appearPoint = data.appearPoint;
        disappearPoint = data.disappearPoint;

        currentState = State.Ready;
        lastActionUpdateTime = Time.time;
    }

    protected override void UpdateActor()
    {
        switch (currentState)
        {
            case State.None:
                break;
            case State.Ready:
                UpdateReady();
                break;
            case State.Dead:
                break;
            case State.Appear:
            case State.Disappear:
                UpdateSpeed();
                UpdateMove();
                break;
            case State.Battle:
                UpdateBattle();
                break;
        }
    }

    protected override void OnDead(Actor killer)
    {
        base.OnDead(killer);

        SystemManager.Instance.GamePointAccumulator.Accumulate(gamePoint);
        SystemManager.Instance.EnemyManager.RemoveEnemy(this);

        currentState = State.Dead;
    }

    protected override void DecreaseHp(Actor attacker, int value, Vector3 damagePos)
    {
        base.DecreaseHp(attacker, value, damagePos);

        Vector3 damagePoint = damagePos + Random.insideUnitSphere * 0.5f;
        SystemManager.Instance.DamageManager.Generate(DamageManager.EnemyDamageIndex, damagePoint, value, Color.magenta);
    }

    private void UpdateReady()
    {
        if (Time.time - lastActionUpdateTime > 1.0f)
        {
            Appear(appearPoint);
        }
    }

    private void UpdateSpeed()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, (Time.time - moveStartTime) / maxSpeedTime);
    }

    private void UpdateMove()
    {
        float distance = Vector3.Distance(targetPosition, transform.position);
        if (distance == 0f)
        {
            Arrived();
            return;
        }

        currentVelocity = (targetPosition - transform.position).normalized * currentSpeed;

        // 속도 = 거리 / 시간 이므로 시간 = 거리 / 속도
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, distance / currentSpeed, maxSpeed);
    }

    private void UpdateBattle()
    {
        if (Time.time - lastActionUpdateTime > 1.0f)
        {
            if (fireRemainCount > 0)
            {
                Fire();

                fireRemainCount--;
            }
            else
            {
                Disappear(disappearPoint);
            }

            lastActionUpdateTime = Time.time;
        }
    }

    private void Arrived()
    {
        currentSpeed = 0f;

        if (currentState == State.Appear)
        {
            currentState = State.Battle;
            lastActionUpdateTime = Time.time;
        }
        else // if (currentState == State.Disappear)
        {
            currentState = State.None;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player)
        {
            if (!player.IsDead)
            {
                BoxCollider box = ((BoxCollider)other);
                Vector3 crashPos = player.transform.position + box.center;
                crashPos.x += box.size.x * 0.5f;

                player.OnCrash(player, damage, crashPos);
            }
        }
    }

    private void Disappear(Vector3 targetPos)
    {
        targetPosition = targetPos;
        currentSpeed = 0f;

        currentState = State.Disappear;
        moveStartTime = Time.time;
    }
}
