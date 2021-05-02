using UnityEngine;

public enum OwnerSide : int
{
    Player = 0,
    Enemy
}

public class Bullet : MonoBehaviour
{
    private const float lifeTime = 15.0f;

    private OwnerSide ownerSide = OwnerSide.Player;
    private float firedTime = 0.0f;
    private bool needMove = false;
    private bool hited = false;
    private int damage = 1;

    [SerializeField] private Vector3 moveDirection = Vector3.zero;
    [SerializeField] private float speed = 0.0f;

    public string FilePath { get; set; }

    public void Fire(OwnerSide ownerSide, Vector3 firePosition, Vector3 direction, float speed, int damage)
    {
        this.ownerSide = ownerSide;
        transform.position = firePosition;
        moveDirection = direction;
        this.speed = speed;
        this.damage = damage;

        needMove = true;
        firedTime = Time.time;
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
        if (!needMove)
        {
            return;
        }

        Vector3 moveVector = moveDirection.normalized * speed * Time.deltaTime;
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
        if (hited)
        {
            return;
        }

        if (collider.gameObject.layer == LayerMask.NameToLayer("EnemyBullet") ||
            collider.gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
        {
            return;
        }

        Actor actor = collider.GetComponentInParent<Actor>();
        if (actor && actor.IsDead)
        {
            return;
        }

        actor.OnBulletHited(actor, damage, transform.position);

        Collider myCollider = GetComponentInChildren<Collider>();
        myCollider.enabled = false;

        hited = true;
        needMove = false;

        GameObject go = SystemManager.Instance.EffectManager.GenerateEffect(EffectManager.BulletDisappearFxIndex, transform.position);
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
        else if (Time.time - firedTime > lifeTime)
        {
            Disappear();
            return true;
        }

        return false;
    }

    private void Disappear()
    {
        Destroy(gameObject);
    }
}
