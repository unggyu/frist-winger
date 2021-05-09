using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

public class Player : Actor
{
    private readonly InputController inputController = new InputController();

    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float bulletSpeed = 1.0f;
    [SerializeField] private Transform fireTransform = null;
    [SerializeField] private BoxCollider boxCollider = null;

    /// <summary>
    /// Host 플레이어인지 여부
    /// </summary>
    private readonly NetworkVariable<bool> isHost =
        new NetworkVariable<bool>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.ServerOnly });

    /// <summary>
    /// 이동 벡터
    /// </summary>
    [SerializeField]
    private Vector3 moveVector = Vector3.zero;

    public void ProcessInput(Vector3 moveDirection)
    {
        if (moveDirection != Vector3.zero)
        {
            moveVector = moveDirection * speed * Time.deltaTime;
        }
    }

    public void Fire()
    {
        Bullet bullet = SystemManager
            .Instance
            .GetCurrentSceneMain<InGameSceneMain>()
            .BulletManager
            .Generate(BulletManager.PlayerBulletIndex);

        bullet.Fire(this, fireTransform.position, fireTransform.right, bulletSpeed, damage.Value);
    }

    public void UpdateClientInput()
    {
        if (IsLocalPlayer)
        {
            inputController.UpdateInput();
        }
    }

    protected override void Initialize()
    {
        base.Initialize();

        if (IsServer && IsLocalPlayer)
        {
            isHost.Value = true;
        }

        PlayerStatePanel playerStatePanel = PanelManager.GetPanel(typeof(PlayerStatePanel)) as PlayerStatePanel;
        playerStatePanel.SetHp(currentHp.Value, maxHp.Value);

        InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();

        if (IsLocalPlayer)
        {
            inGameSceneMain.Player = this;
        }

        Transform startTransform;
        if (isHost.Value)
        {
            startTransform = inGameSceneMain.PlayerStartTransform1;
        }
        else
        {
            startTransform = inGameSceneMain.PlayerStartTransform2;
        }

        SetPosition(startTransform.position);

        if (actorInstanceId.Value != 0)
        {
            // Player 등록
            inGameSceneMain.ActorManager.Regist(actorInstanceId.Value, this);
        }
    }

    protected override void UpdateActor()
    {
        UpdateClientInput();
        UpdateMove();
    }

    protected override void DecreaseHp(Actor attacker, int value, Vector3 damagePos)
    {
        base.DecreaseHp(attacker, value, damagePos);
        PlayerStatePanel playerStatePanel = PanelManager.GetPanel(typeof(PlayerStatePanel)) as PlayerStatePanel;
        playerStatePanel.SetHp(currentHp.Value, maxHp.Value);

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

                enemy.OnCrash(enemy, damage.Value, crashPos);
            }
        }
    }

    private void UpdateMove()
    {
        if (moveVector.sqrMagnitude == 0)
        {
            return;
        }

        if (IsServer)
        {
            MoveClientRpc(moveVector);
        }
        else
        {
            MoveServerRpc(moveVector);
            if (IsLocalPlayer)
            {
                transform.position += AdjustMoveVector(moveVector);
                moveVector = Vector3.zero;
            }
        }
    }

    [ServerRpc]
    private void MoveServerRpc(Vector3 moveVector)
    {
        this.moveVector = moveVector;
        transform.position += AdjustMoveVector(moveVector);
        this.moveVector = Vector3.zero;
    }

    [ClientRpc]
    private void MoveClientRpc(Vector3 moveVector)
    {
        this.moveVector = moveVector;
        transform.position += AdjustMoveVector(moveVector);
        this.moveVector = Vector3.zero;
    }

    private Vector3 AdjustMoveVector(Vector3 moveVector)
    {
        InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
        if (inGameSceneMain != null)
        {
            Transform mainBGQuadTransform = inGameSceneMain.MainBGQuadTransform;
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
        }

        return moveVector;
    }
}
