// #define NETWORK_BEHAVIOUR // 정상적으로 NetworkBehaviour 인스턴스의 Update로 호출되어 실행되고 있을 때
#define MONO_BEHAVIOUR // MohoBehaviour 인스턴스의 Update로 호출되어 실행되고 있을 때
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

public class Player : Actor
{
    private readonly InputController inputController = new InputController();

    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float bulletSpeed = 1.0f;
    [SerializeField] private NetworkVariable<Vector3> moveVector = new NetworkVariable<Vector3>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, Vector3.zero);
    [SerializeField] private Transform fireTransform = null;
    [SerializeField] private BoxCollider boxCollider = null;

    public void ProcessInput(Vector3 moveDirection)
    {
        moveVector.Value = moveDirection * speed * Time.deltaTime;
    }

    public void Fire()
    {
        Bullet bullet = SystemManager
            .Instance
            .GetCurrentSceneMain<InGameSceneMain>()
            .BulletManager
            .Generate(BulletManager.PlayerBulletIndex);

        bullet.Fire(this, fireTransform.position, fireTransform.right, bulletSpeed, damage);
    }

    public void UpdateClientInput()
    {
        if (IsClient)
        {
            inputController.UpdateInput();
        }
    }

    protected override void Initialize()
    {
        base.Initialize();

        InGameSceneMain sceneMain;
        if ((sceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>()) != null)
        {
            InitializePrivate();
        }
        else
        {
            SystemManager.Instance.CurrentSceneMainChanged += CurrentSceneMainChanged;
        }
    }

    protected override void UpdateActor()
    {
        UpdateMove();
    }

    protected override void DecreaseHp(Actor attacker, int value, Vector3 damagePos)
    {
        base.DecreaseHp(attacker, value, damagePos);
        PlayerStatePanel playerStatePanel = PanelManager.GetPanel(typeof(PlayerStatePanel)) as PlayerStatePanel;
        playerStatePanel.SetHp(currentHp, maxHp);

        Vector3 damagePoint = damagePos + Random.insideUnitSphere * 0.5f;
        SystemManager
            .Instance
            .GetCurrentSceneMain<InGameSceneMain>()
            .DamageManager
            .Generate(DamageManager.PlayerDamageIndex, damagePoint, value, Color.red);
    }

    protected override void OnDead(Actor killer)
    {
        base.OnDead(killer);
        gameObject.SetActive(false);
    }

    private void CurrentSceneMainChanged(object sender, string sceneName)
    {
        Debug.Log("CurrentSceneMainChanged sceneName = " + sceneName);

        if (sceneName.Equals(nameof(InGameSceneMain)))
        {
            InitializePrivate();
        }

        SystemManager.Instance.CurrentSceneMainChanged -= CurrentSceneMainChanged;
    }

    private void InitializePrivate()
    {
        PlayerStatePanel playerStatePanel = PanelManager.GetPanel(typeof(PlayerStatePanel)) as PlayerStatePanel;
        playerStatePanel.SetHp(currentHp, maxHp);

        InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();

        if (IsLocalPlayer)
        {
            inGameSceneMain.Player = this;
        }

        Transform startTransform;
        if (IsServer)
        {
            startTransform = inGameSceneMain.PlayerStartTransform1;
        }
        else
        {
            startTransform = inGameSceneMain.PlayerStartTransform2;
        }

        SetPosition(startTransform.position);
    }

    private void Update()
    {
        UpdateClientInput();
        UpdateMove();
    }

    private void UpdateMove()
    {
        if (moveVector.Value.sqrMagnitude == 0)
        {
            return;
        }

#if NETWORK_BEHAVIOUR
        MoveServerRpc(moveVector.Value);
#elif MONO_BEHAVIOUR
        if (IsServer)
        {
            // Host 플레이어인 경우 클라이언트에게 RPC로 보냄
            MoveClientRpc(moveVector.Value);
        }
        else
        {
            // Client 플레리어인 경우 서버에게 RPC로 보내고
            // Self 동작 수행
            MoveServerRpc(moveVector.Value);
            if (IsLocalPlayer)
            {
                transform.position += AdjustMoveVector(moveVector.Value);
            }
        }
#endif
    }

    private void Move(Vector3 moveVector)
    {
        this.moveVector.Value = moveVector;
        transform.position += moveVector;
    }

    private void SetPosition(Vector3 position)
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

    [ServerRpc]
    private void MoveServerRpc(Vector3 moveVector)
    {
        this.moveVector.Value = moveVector;
        transform.position += moveVector;
        this.moveVector.Value = Vector3.zero;
    }

    [ClientRpc]
    private void MoveClientRpc(Vector3 moveVector)
    {
        this.moveVector.Value = moveVector;
        transform.position += moveVector;
        this.moveVector.Value = Vector3.zero;
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

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy)
        {
            if (!enemy.IsDead)
            {
                BoxCollider box = ((BoxCollider)other);
                Vector3 crashPos = enemy.transform.position + box.center;
                crashPos.x += box.size.x * 0.5f;

                enemy.OnCrash(enemy, damage, crashPos);
            }
        }
    }

    private Vector3 AdjustMoveVector(Vector3 moveVector)
    {
        Transform mainBGQuadTransform = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().MainBGQuadTransform;
        Vector3 result = boxCollider.transform.position + boxCollider.center + moveVector;

        if (result.x - boxCollider.size.x * 0.5f < -mainBGQuadTransform.localScale.x * 0.5f)
        {
            moveVector.x = 0;
        }

        if (result.x + boxCollider.size.x * 0.5f > mainBGQuadTransform.localScale.x * 0.5f)
        {
            moveVector.x = 0;
        }

        if (result.y - boxCollider.size.y * 0.5f < -mainBGQuadTransform.localScale.y * 0.5f)
        {
            moveVector.y = 0;
        }

        if (result.y + boxCollider.size.y * 0.5f > mainBGQuadTransform.localScale.y * 0.5f)
        {
            moveVector.y = 0;
        }

        return moveVector;
    }
}
