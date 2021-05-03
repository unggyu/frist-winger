using UnityEngine;

[System.Serializable]
public class EnemyGenerateData
{
    /// <summary>
    /// 적 파일 경로
    /// </summary>
    public string filePath;

    /// <summary>
    /// 최대 hp
    /// </summary>
    public int maxHp;

    /// <summary>
    /// 총알 데미지
    /// </summary>
    public int damage;

    /// <summary>
    /// 충돌 데미지
    /// </summary>
    public int crashDamage;

    public float bulletSpeed;
    public int fireRemainCount;
    public int gamePoint;

    /// <summary>
    /// 입장 전 생성되는 위치
    /// </summary>
    public Vector3 generatePoint;

    /// <summary>
    /// 입장 시 도착 위치
    /// </summary>
    public Vector3 appearPoint;

    /// <summary>
    /// 퇴장 시 목표 위치
    /// </summary>
    public Vector3 disappearPoint;
}

public class Squadron : MonoBehaviour
{
    [SerializeField] EnemyGenerateData[] enemyGenerateDatas = null;

    public void GenerateAllData()
    {
        for (int i = 0; i < enemyGenerateDatas.Length; i++)
        {
            SystemManager.Instance.EnemyManager.GenerateEnemy(enemyGenerateDatas[i]);
        }
    }
}
