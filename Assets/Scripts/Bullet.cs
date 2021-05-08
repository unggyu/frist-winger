using MLAPI;
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
    /// 위치
    /// </summary>
    [SerializeField]
    private readonly NetworkVariable<Vector3> position =
        new NetworkVariable<Vector3>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });

    /// <summary>
    /// 활성화 여부
    /// </summary>
    [SerializeField]
    private readonly NetworkVariable<bool> isActive =
        new NetworkVariable<bool>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });

    /// <summary>
    /// Prefab 파일 경로
    /// </summary>
    private readonly NetworkVariable<string> filePath =
        new NetworkVariable<string>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });

    /// <summary>
    /// 해당 총알을 발사한 객체. NetworkBehaviour 상속 클래스라 NetworkVariable 안 됨
    /// </summary>
    [SerializeField] private Actor owner;

    /// <summary>
    /// Prefab 파일 경로
    /// </summary>
    public string FilePath
    {
        get => filePath.Value;
        set => filePath.Value = value;
    }

    public void Fire(Actor owner, Vector3 firePosition, Vector3 direction, float speed, int damage)
    {
        this.owner = owner;
        SetPosition(firePosition);
        moveDirection.Value = direction;
        this.speed.Value = speed;
        this.damage.Value = damage;

        needMove.Value = true;
        firedTime.Value = Time.time;
    }

    public void SetActive(bool active)
    {
        isActive.Value = active;
    }

    private void Start()
    {
        if (NetworkManager.IsConnectedClient)
        {
            InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
            transform.SetParent(inGameSceneMain.BulletManager.transform);
            inGameSceneMain.BulletCacheSystem.Add(FilePath, gameObject);
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        position.OnValueChanged += OnPositionChanged;
        isActive.OnValueChanged += OnIsActiveChanged;
    }

    private void OnDisable()
    {
        position.OnValueChanged -= OnPositionChanged;
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

    private void UpdateMove()
    {
        if (!needMove.Value)
        {
            return;
        }

        Vector3 moveVector = moveDirection.Value.normalized * speed.Value * Time.deltaTime;
        moveVector = AdjustMove(moveVector);
        transform.position += moveVector;
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

        Actor actor = collider.GetComponentInParent<Actor>();
        if (actor && actor.IsDead || actor.gameObject.layer == owner.gameObject.layer)
        {
            return;
        }

        actor.OnBulletHited(actor, damage.Value, transform.position);

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

    private void OnTriggerEnter(Collider other)
    {
        OnBulletCollision(other);
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

    private void Disappear()
    {
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Remove(this);
    }

    private void SetPosition(Vector3 position)
    {
        this.position.Value = position;
    }

    private void OnPositionChanged(Vector3 previousPosition, Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    private void OnIsActiveChanged(bool previousValue, bool newValue)
    {
        gameObject.SetActive(newValue);
    }
}
