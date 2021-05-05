using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

public class Player : Actor
{
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float bulletSpeed = 1.0f;
    [SerializeField] private NetworkVariable<Vector3> moveVector = new NetworkVariable<Vector3>(Vector3.zero);
    [SerializeField] private Transform fireTransform = null;
    [SerializeField] private BoxCollider boxCollider = null;
    [SerializeField] private NetworkObject networkObject = null;

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

    protected override void Initialize()
    {
        base.Initialize();

        InGameSceneMain sceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
        if (sceneMain != null)
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
        InGameSceneMain sceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
        if (sceneMain != null)
        {
            PlayerStatePanel playerStatePanel = PanelManager.GetPanel(typeof(PlayerStatePanel)) as PlayerStatePanel;
            playerStatePanel.SetHp(currentHp, maxHp);

            if (IsLocalPlayer)
            {
                SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().Player = this;
            }
        }
    }

    private void Update()
    {
        UpdateMove();
    }

    private void UpdateMove()
    {
        if (moveVector.Value.sqrMagnitude == 0)
        {
            return;
        }

        moveVector.Value = AdjustMoveVector(moveVector.Value);

        MoveServerRpc(moveVector.Value);
    }

    [ServerRpc]
    private void MoveServerRpc(Vector3 moveVector)
    {
        this.moveVector.Value = moveVector;
        transform.position += moveVector;
        // SetDirtyBit(1);
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
