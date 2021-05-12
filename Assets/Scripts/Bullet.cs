using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

public enum OwnerSide : int
{
    Player = 0,
    Enemy
}

public class Bullet : NetworkBehaviour
{
    /// <summary>
    /// 총알 생존시간
    /// </summary>
    private const float lifeTime = 15.0f;

    /// <summary>
    /// 활성화 여부
    /// </summary>
    private readonly NetworkVariable<bool> isActive =
        new NetworkVariable<bool>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.ServerOnly }, true);

    /// <summary>
    /// 이동이 필요한지 여부
    /// </summary>
    private readonly NetworkVariable<bool> needMove =
        new NetworkVariable<bool>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, false);

    /// <summary>
    /// 발사된 시각
    /// </summary>
    private readonly NetworkVariable<float> firedTime =
        new NetworkVariable<float>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, 0.0f);

    /// <summary>
    /// 부딪혔는지 플래그
    /// </summary>
    private readonly NetworkVariable<bool> hited =
        new NetworkVariable<bool>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, false);

    /// <summary>
    /// 데미지
    /// </summary>
    private readonly NetworkVariable<int> damage =
        new NetworkVariable<int>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, 1);

    /// <summary>
    /// 이동 방향
    /// </summary>
    [SerializeField]
    private readonly NetworkVariable<Vector3> moveDirection = 
        new NetworkVariable<Vector3>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, Vector3.zero);

    /// <summary>
    /// 속도
    /// </summary>
    [SerializeField]
    private readonly NetworkVariable<float> speed =
        new NetworkVariable<float>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, 0.0f);

    /// <summary>
    /// Prefab 파일 경로
    /// </summary>
    private readonly NetworkVariable<string> filePath =
        new NetworkVariable<string>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });

    /// <summary>
    /// 발사자 객체 Id
    /// </summary>
    private readonly NetworkVariable<int> ownerInstanceId =
        new NetworkVariable<int>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });

    /// <summary>
    /// 초기화 여부
    /// </summary>
    private bool initialized = false;

    /// <summary>
    /// 활성화 여부
    /// </summary>
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
    /// Prefab 파일 경로
    /// </summary>
    public string FilePath
    {
        get => filePath.Value;
        set => filePath.Value = value;
    }

    public void Fire(int ownerInstanceId, Vector3 firePosition, Vector3 direction, float speed, int damage)
    {
        this.ownerInstanceId.Value = ownerInstanceId;
        SetPosition(firePosition);
        moveDirection.Value = direction;
        this.speed.Value = speed;
        this.damage.Value = damage;

        needMove.Value = true;
        firedTime.Value = Time.time;
    }

    private void Start()
    {
        // InGameSceneMain이 되기전에 Start 메소드가 실행되는 경우가 있음
        if (SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>() != null)
        {
            Initialize();
        }
        else
        {
            SystemManager.Instance.CurrentSceneMainChanged += CurrentSceneMainChanged;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (ProcessDisappearCondition())
        {
            return;
        }

        UpdateMove();
    }

    private void OnDestroy()
    {
        isActive.OnValueChanged -= OnIsActiveChanged;
    }

    private void OnTriggerEnter(Collider other)
    {
        OnBulletCollision(other);
    }

    private void Initialize()
    {
        isActive.OnValueChanged += OnIsActiveChanged;

        if (!initialized && !IsServer)
        {
            InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
            transform.SetParent(inGameSceneMain.BulletManager.transform);
            inGameSceneMain.BulletCacheSystem.Add(FilePath, gameObject);
            gameObject.SetActive(false);
        }

        initialized = true;
    }

    private void OnBulletCollision(Collider collider)
    {
        if (hited.Value)
        {
            return;
        }

        if (collider.gameObject.layer == LayerMask.NameToLayer("EnemyBullet") ||
            collider.gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
        {
            return;
        }

        Actor owner = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ActorManager.GetActor(ownerInstanceId.Value);
        Actor actor = collider.GetComponentInParent<Actor>();
        if (actor && actor.IsDead || actor.gameObject.layer == owner.gameObject.layer)
        {
            return;
        }

        actor.OnBulletHited(damage.Value, transform.position);

        Collider myCollider = GetComponentInChildren<Collider>();
        myCollider.enabled = false;

        hited.Value = true;
        needMove.Value = false;

        GameObject go = SystemManager
            .Instance
            .GetCurrentSceneMain<InGameSceneMain>()
            .EffectManager
            .GenerateEffect(EffectManager.BulletDisappearFxIndex, transform.position);
        go.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        Disappear();
    }

    private void UpdateMove()
    {
        if (IsHost && needMove.Value) // 총알의 움직임은 오직 서버에서만 제어
        {
            Vector3 moveVector = moveDirection.Value.normalized * speed.Value * Time.deltaTime;
            moveVector = AdjustMove(moveVector);
            MoveClientRpc(moveVector);
        }
    }

    private Vector3 AdjustMove(Vector3 moveVector)
    {
        if (Physics.Linecast(transform.position, transform.position + moveVector, out RaycastHit hitInfo))
        {
            moveVector = hitInfo.point - transform.position;
            OnBulletCollision(hitInfo.collider);
        }

        return moveVector;
    }

    private bool ProcessDisappearCondition()
    {
        if (transform.position.x > 15.0f ||
            transform.position.x < -15.0f ||
            transform.position.y > 15.0f ||
            transform.position.y < -15.0f)
        {
            Disappear();
            return true;
        }
        else if (Time.time - firedTime.Value > lifeTime)
        {
            Disappear();
            return true;
        }

        return false;
    }

    private void SetPosition(Vector3 position)
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

    private void CurrentSceneMainChanged(object sender, string sceneName)
    {
        if (sceneName.Equals(nameof(InGameSceneMain)))
        {
            if (!initialized)
            {
                Initialize();
            }
        }
    }

    private void Disappear()
    {
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Remove(this);
    }

    private void OnIsActiveChanged(bool previousValue, bool newValue)
    {
        if (gameObject.activeSelf != newValue)
        {
            gameObject.SetActive(newValue);
        }
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

    [ClientRpc]
    private void MoveClientRpc(Vector3 moveVector)
    {
        transform.position += moveVector;
    }

    [ClientRpc]
    private void SetActiveClientRpc(bool value)
    {
        gameObject.SetActive(value);
    }
}
