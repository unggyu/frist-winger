using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum State
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateSpeed()
    {

    }

    void UpdateMove()
    {

    }

    void Arrived()
    {

    }

    public void Appear(Vector3 targetPos)
    {
        targetPosition = targetPos;
        currentSpeed = maxSpeed;

        currentState = State.Appear;
    }

    void Disappear(Vector3 targetPos)
    {
        targetPosition = targetPos;
        currentSpeed = 0;

        currentState = State.Disappear;
    }
}
