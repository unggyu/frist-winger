﻿using UnityEngine;

public class Player : Actor
{
    [SerializeField] Vector3 moveVector = Vector3.zero;
    [SerializeField] float speed = 1.0f;
    [SerializeField] BoxCollider boxCollider = null;
    [SerializeField] Transform mainBGQuadTransform = null;
    [SerializeField] Transform fireTransform = null;
    [SerializeField] float bulletSpeed = 1.0f;

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
            enemy.OnCrash(enemy, damage);
        }
    }

    public void Fire()
    {
        Bullet bullet = SystemManager.Instance.BulletManager.Generate(BulletManager.PlayerBulletIndex);
        bullet.Fire(OwnerSide.Player, fireTransform.position, fireTransform.right, bulletSpeed, damage);
    }

    protected override void OnDead(Actor killer)
    {
        base.OnDead(killer);
        gameObject.SetActive(false);
    }
}
