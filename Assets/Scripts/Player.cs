using UnityEngine;

public class Player : Actor
{
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float bulletSpeed = 1.0f;
    [SerializeField] private Vector3 moveVector = Vector3.zero;
    [SerializeField] private Transform mainBGQuadTransform = null;
    [SerializeField] private Transform fireTransform = null;
    [SerializeField] private BoxCollider boxCollider = null;

    public void ProcessInput(Vector3 moveDirection)
    {
        moveVector = moveDirection * speed * Time.deltaTime;
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
        PlayerStatePanel playerStatePanel = PanelManager.GetPanel(typeof(PlayerStatePanel)) as PlayerStatePanel;
        playerStatePanel.SetHp(currentHp, maxHp);
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

    private void Update()
    {
        UpdateMove();
    }

    private void UpdateMove()
    {
        if (moveVector.sqrMagnitude == 0)
        {
            return;
        }

        moveVector = AdjustMoveVector(moveVector);

        transform.position += moveVector;
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
