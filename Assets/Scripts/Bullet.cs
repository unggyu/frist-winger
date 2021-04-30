using UnityEngine;

public enum OwnerSide : int
{
    Player = 0,
    Enemy
}

public class Bullet : MonoBehaviour
{
    OwnerSide ownerSide = OwnerSide.Player;

    [SerializeField]
    Vector3 moveDirection = Vector3.zero;

    [SerializeField]
    float speed = 0.0f;

    bool needMove = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMove();
    }

    void UpdateMove()
    {
        if (!needMove)
        {
            return;
        }

        Vector3 moveVector = moveDirection.normalized * speed * Time.deltaTime;
        transform.position += moveVector;
    }

    public void Fire(OwnerSide ownerSide, Vector3 firePosition, Vector3 direction, float speed)
    {
        this.ownerSide = ownerSide;
        transform.position = firePosition;
        moveDirection = direction;
        this.speed = speed;

        needMove = true;
    }
}
