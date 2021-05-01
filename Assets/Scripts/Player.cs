using UnityEngine;

public class Player : Actor
{
    [SerializeField] Vector3 moveVector = Vector3.zero;
    [SerializeField] float speed = 1.0f;
    [SerializeField] BoxCollider boxCollider = null;
    [SerializeField] Transform mainBGQuadTransform = null;
    [SerializeField] Transform fireTransform = null;
    [SerializeField] GameObject bullet = null;
    [SerializeField] float bulletSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMove();
    }

    protected override void UpdateActor()
    {
        UpdateMove();
    }

    void UpdateMove()
    {
        if (moveVector.sqrMagnitude == 0)
        {
            return;
        }

        moveVector = AdjustMoveVector(moveVector);

        transform.position += moveVector;
    }

    public void ProcessInput(Vector3 moveDirection)
    {
        moveVector = moveDirection * speed * Time.deltaTime;
    }

    Vector3 AdjustMoveVector(Vector3 moveVector)
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

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy)
        {
            enemy.OnCrash(damage);
        }
    }

    public void Fire()
    {
        GameObject go = Instantiate(this.bullet);
        Bullet bullet = go.GetComponent<Bullet>();
        bullet.Fire(OwnerSide.Player, fireTransform.position, fireTransform.right, bulletSpeed, damage);
    }
}
