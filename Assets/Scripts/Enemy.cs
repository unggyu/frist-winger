﻿using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum State : int
    {
        /// <summary>
        /// 사용전
        /// </summary>
        None = -1,

        /// <summary>
        /// 준비완료
        /// </summary>
        Ready = 0,

        /// <summary>
        /// 등장
        /// </summary>
        Appear,

        /// <summary>
        /// 전투중
        /// </summary>
        Battle,

        /// <summary>
        /// 사망
        /// </summary>
        Dead,

        /// <summary>
        /// 퇴장
        /// </summary>
        Disappear
    }

    [SerializeField]
    State currentState = State.None;

    const float maxSpeed = 10.0f;
    const float maxSpeedTime = 0.5f;

    [SerializeField]
    Vector3 targetPosition = Vector3.zero;

    [SerializeField]
    float currentSpeed = 0.0f;

    Vector3 currentVelocity = Vector3.zero;

    float moveStartTime = 0.0f;
    float battleStartTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Appear(new Vector3(7f, transform.position.y, transform.position.z));
        }

        switch (currentState)
        {
            case State.None:
            case State.Ready:
                break;
            case State.Dead:
                break;
            case State.Appear:
            case State.Disappear:
                UpdateSpeed();
                UpdateMove();
                break;
            case State.Battle:
                UpdateBattle();
                break;
        }
    }

    void UpdateSpeed()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, (Time.time - moveStartTime) / maxSpeedTime);
    }

    void UpdateMove()
    {
        float distance = Vector3.Distance(targetPosition, transform.position);
        if (distance == 0f)
        {
            Arrived();
            return;
        }

        currentVelocity = (targetPosition - transform.position).normalized * currentSpeed;

        // 속도 = 거리 / 시간 이므로 시간 = 거리 / 속도
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, distance / currentSpeed, maxSpeed);
    }

    void Arrived()
    {
        currentSpeed = 0f;

        if (currentState == State.Appear)
        {
            currentState = State.Battle;
            battleStartTime = Time.time;
        }
        else // if (currentState == State.Disappear)
        {
            currentState = State.None;
        }
    }

    public void Appear(Vector3 targetPos)
    {
        targetPosition = targetPos;
        currentSpeed = maxSpeed;

        currentState = State.Appear;
        moveStartTime = Time.time;
    }

    void Disappear(Vector3 targetPos)
    {
        targetPosition = targetPos;
        currentSpeed = 0f;

        currentState = State.Disappear;
        moveStartTime = Time.time;
    }

    void UpdateBattle()
    {
        if (Time.time - battleStartTime > 3.0f)
        {
            Disappear(new Vector3(-15.0f, transform.position.y, transform.position.z));
        }
    }
}