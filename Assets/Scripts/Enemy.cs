using MLAPI.NetworkVariable;
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

    /// <summary>
    /// 최고 속도
    /// </summary>
    private const float maxSpeed = 10.0f;

    /// <summary>
    /// 최고 속도에 이르는 시간
    /// </summary>
    private const float maxSpeedTime = 0.5f;

    /// <summary>
    /// 입장 시 도착 위치
    /// </summary>
    private readonly NetworkVariable<Vector3> appearPoint =
        new NetworkVariable<Vector3>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, Vector3.zero);

    /// <summary>
    /// 퇴장 시 목표 위치
    /// </summary>
    private readonly NetworkVariable<Vector3> disappearPoint =
        new NetworkVariable<Vector3>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, Vector3.zero);

    /// <summary>
    /// 마지막으로 업데이트한 시간
    /// </summary>
    private readonly NetworkVariable<float> lastActionUpdateTime =
        new NetworkVariable<float>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, 0.0f);

    /// <summary>
    /// 이동 시작 시간
    /// </summary>
    private readonly NetworkVariable<float> moveStartTime =
        new NetworkVariable<float>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, 0.0f);

    /// <summary>
    /// 방향을 고려한 속도 벡터
    /// </summary>
    private readonly NetworkVariable<Vector3> currentVelocity =
        new NetworkVariable<Vector3>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, Vector3.zero);

    /// <summary>
    /// 현재 상태 값
    /// </summary>
    [SerializeField] private NetworkVariable<State> currentState =
        new NetworkVariable<State>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, State.None);

    /// <summary>
    /// 목표점
    /// </summary>
    [SerializeField] private NetworkVariable<Vector3> targetPosition =
        new NetworkVariable<Vector3>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, Vector3.zero);

    /// <summary>
    /// 현재 속도
    /// </summary>
    [SerializeField] private NetworkVariable<float> currentSpeed =
        new NetworkVariable<float>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, 0.0f);

    [SerializeField] private Transform fireTransform = null;

    /// <summary>
    /// 총알 속도
    /// </summary>
    [SerializeField] private NetworkVariable<float> bulletSpeed =
        new NetworkVariable<float>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, 1.0f);

    /// <summary>
    /// 남은 총알 개수
    /// </summary>
    [SerializeField] private NetworkVariable<int> fireRemainCount =
        new NetworkVariable<int>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, 1);

    /// <summary>
    /// 게임 점수
    /// </summary>
    [SerializeField] private NetworkVariable<int> gamePoint =
        new NetworkVariable<int>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, 0);

    /// <summary>
    /// Prefab 파일 위치
    /// </summary>
    [SerializeField] private NetworkVariable<string> filePath = new NetworkVariable<string>();

    public string FilePath
    {
        get => filePath.Value;
        set => filePath.Value = value;
    }

    public void Appear(Vector3 targetPos)
    {
        targetPosition.Value = targetPos;
        currentSpeed.Value = maxSpeed;

        currentState.Value = State.Appear;
        moveStartTime.Value = Time.time;
    }

    public void Fire()
    {
        Bullet bullet = SystemManager
            .Instance
            .GetCurrentSceneMain<InGameSceneMain>()
            .BulletManager
            .Generate(BulletManager.EnemyBulletIndex);

        bullet.Fire(this, fireTransform.position, -fireTransform.right, bulletSpeed.Value, damage.Value);
    }

    public void Reset(SquadronMemberStruct data)
    {
        EnemyStruct enemyStruct = SystemManager.Instance.EnemyTable.GetEnemy(data.EnemyId);

        currentHp.Value = maxHp.Value = enemyStruct.MaxHp; // currentHp까지 다시 입력
        damage.Value = enemyStruct.Damage; // 총알 데미지
        crashDamage.Value = enemyStruct.CrashDamage; // 충돌 데미지
        bulletSpeed.Value = enemyStruct.BulletSpeed; // 총알 속도
        fireRemainCount.Value = enemyStruct.FireRemainCount; // 발사할 총알 갯수
        gamePoint.Value = enemyStruct.GamePoint; // 파괴 시 얻을 점수

        appearPoint.Value = new Vector3(data.AppearPointX, data.AppearPointY, 0); // 입장 시 도착
        disappearPoint.Value = new Vector3(data.DisappearPointX, data.DisappearPointY, 0); // 퇴장 시 목표

        currentState.Value = State.Ready;
        lastActionUpdateTime.Value = Time.time;
    }

    protected override void Initialize()
    {
        base.Initialize();
        if (NetworkManager.IsConnectedClient)
        {
            InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
            transform.SetParent(inGameSceneMain.EnemyManager.transform);
            inGameSceneMain.EnemyCacheSystem.Add(FilePath, gameObject);
            gameObject.SetActive(true);
        }
    }

    protected override void UpdateActor()
    {
        switch (currentState.Value)
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

        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().GamePointAccumulator.Accumulate(gamePoint.Value);
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.RemoveEnemy(this);

        currentState.Value = State.Dead;
    }

    protected override void DecreaseHp(Actor attacker, int value, Vector3 damagePos)
    {
        base.DecreaseHp(attacker, value, damagePos);

        Vector3 damagePoint = damagePos + Random.insideUnitSphere * 0.5f;
        SystemManager
            .Instance
            .GetCurrentSceneMain<InGameSceneMain>()
            .DamageManager
            .Generate(DamageManager.EnemyDamageIndex, damagePoint, value, Color.magenta);
    }

    private void UpdateReady()
    {
        if (Time.time - lastActionUpdateTime.Value > 1.0f)
        {
            Appear(appearPoint.Value);
        }
    }

    private void UpdateSpeed()
    {
        currentSpeed.Value = Mathf.Lerp(currentSpeed.Value, maxSpeed, (Time.time - moveStartTime.Value) / maxSpeedTime);
    }

    private void UpdateMove()
    {
        float distance = Vector3.Distance(targetPosition.Value, transform.position);
        if (distance == 0f)
        {
            Arrived();
            return;
        }

        this.currentVelocity.Value = (targetPosition.Value - transform.position).normalized * currentSpeed.Value;

        // this.currentVelocity.Value는 ref로 넘길 수 없으므로 임시로 생성
        Vector3 currentVelocity = this.currentVelocity.Value;
        // 속도 = 거리 / 시간 이므로 시간 = 거리 / 속도
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition.Value, ref currentVelocity, distance / currentSpeed.Value, maxSpeed);
        // SmoothDamp에서 계산된 currentVelocity를 주입
        this.currentVelocity.Value = currentVelocity;
    }

    private void UpdateBattle()
    {
        if (Time.time - lastActionUpdateTime.Value > 1.0f)
        {
            if (fireRemainCount.Value > 0)
            {
                Fire();

                fireRemainCount.Value--;
            }
            else
            {
                Disappear(disappearPoint.Value);
            }

            lastActionUpdateTime.Value = Time.time;
        }
    }

    private void Arrived()
    {
        currentSpeed.Value = 0.0f;

        if (currentState.Value == State.Appear)
        {
            currentState.Value = State.Battle;
            lastActionUpdateTime.Value = Time.time;
        }
        else // if (currentState == State.Disappear)
        {
            currentState.Value = State.None;
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

                player.OnCrash(player, damage.Value, crashPos);
            }
        }
    }

    private void Disappear(Vector3 targetPos)
    {
        targetPosition.Value = targetPos;
        currentSpeed.Value = 0.0f;

        currentState.Value = State.Disappear;
        moveStartTime.Value = Time.time;
    }
}
