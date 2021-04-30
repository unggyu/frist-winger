using UnityEngine;

public enum OwnerSide : int
{
    Player = 0,
    Enemy
}

public class Bullet : MonoBehaviour
{
    const float lifeTime = 15.0f;

    OwnerSide ownerSide = OwnerSide.Player;

    [SerializeField]
    Vector3 moveDirection = Vector3.zero;

    [SerializeField]
    float speed = 0.0f;

    float firedTime = 0.0f;
    bool needMove = false;
    bool hited = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ProcessDisappearCondition())
        {
            return;
        }

        UpdateMove();
    }

    void UpdateMove()
    {
        if (!needMove)
        {
            return;
        }

        Vector3 moveVector = moveDirection.normalized * speed * Time.deltaTime;
        moveVector = AdjustMove(moveVector);
        transform.position += moveVector;
        Debug.Log(transform.position);
    }

    public void Fire(OwnerSide ownerSide, Vector3 firePosition, Vector3 direction, float speed)
    {
        this.ownerSide = ownerSide;
        transform.position = firePosition;
        moveDirection = direction;
        this.speed = speed;

        needMove = true;
        firedTime = Time.time;
    }

    Vector3 AdjustMove(Vector3 moveVector)
    {
        if (Physics.Linecast(transform.position, transform.position + moveVector, out RaycastHit hitInfo))
        {
            moveVector = hitInfo.point - transform.position;
            OnBulletCollision(hitInfo.collider);
        }

        return moveVector;
    }

    void OnBulletCollision(Collider collider)
    {
        if (hited)
        {
            return;
        }

        hited = true;
        needMove = false;

        Collider myCollider = GetComponentInChildren<Collider>();
        myCollider.enabled = false;

        if (ownerSide == OwnerSide.Player)
        {
            Enemy enemy = collider.GetComponentInParent<Enemy>();
        }
        else
        {
            Player player = collider.GetComponentInParent<Player>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        OnBulletCollision(other);
    }

    bool ProcessDisappearCondition()
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

    void Disappear()
    {
        Destroy(gameObject);
    }
}
