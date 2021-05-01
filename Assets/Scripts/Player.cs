using UnityEngine;

public class Player : Actor
{
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float bulletSpeed = 1.0f;
    [SerializeField] private Vector3 moveVector = Vector3.zero;
    [SerializeField] private Transform mainBGQuadTransform = null;
    [SerializeField] private Transform fireTransform = null;
    [SerializeField] private BoxCollider boxCollider = null;
    [SerializeField] private Gage hpGage = null;

    public void ProcessInput(Vector3 moveDirection)
    {
        moveVector = moveDirection * speed * Time.deltaTime;
    }

    public void Fire()
    {
        Bullet bullet = SystemManager.Instance.BulletManager.Generate(BulletManager.PlayerBulletIndex);
        bullet.Fire(OwnerSide.Player, fireTransform.position, fireTransform.right, bulletSpeed, damage);
    }

    protected override void Initialize()
    {
        base.Initialize();
        hpGage.SetHp(currentHp, maxHp);
    }

    protected override void UpdateActor()
    {
        UpdateMove();
    }

    protected override void DecreaseHp(Actor attacker, int value)
    {
        base.DecreaseHp(attacker, value);
        hpGage.SetHp(currentHp, maxHp);
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
            enemy.OnCrash(enemy, damage);
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
