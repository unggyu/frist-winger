using MLAPI;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private readonly List<Enemy> enemies = new List<Enemy>();

    [SerializeField] EnemyFactory enemyFactory = null;
    [SerializeField] PrefabCacheData[] enemyFiles = null;

    public List<Enemy> Enemies => enemies;

    public bool GenerateEnemy(SquadronMemberStruct data)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return false;
        }

        string filePath = SystemManager.Instance.EnemyTable.GetEnemy(data.EnemyId).FilePath;
        GameObject go = SystemManager
            .Instance
            .GetCurrentSceneMain<InGameSceneMain>()
            .EnemyCacheSystem
            .Archive(filePath);

        Enemy enemy = go.GetComponent<Enemy>();
        Vector3 position = new Vector3(data.GeneratePointX, data.GeneratePointY, 0);
        enemy.SetPosition(position);
        enemy.ResetData(data);
        enemies.Add(enemy);

        return true;
    }

    public bool RemoveEnemy(Enemy enemy)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return false;
        }

        if (!enemies.Contains(enemy))
        {
            Debug.LogError("No exists Enemy");
            return false;
        }

        enemies.Remove(enemy);
        SystemManager
            .Instance
            .GetCurrentSceneMain<InGameSceneMain>()
            .EnemyCacheSystem
            .Restore(enemy.FilePath, enemy.gameObject);

        return true;
    }

    public void Prepare()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        for (int i = 0; i < enemyFiles.Length; i++)
        {
            GameObject go = enemyFactory.Load(enemyFiles[i].filePath);
            InGameSceneMain inGameSceneMain;
            if ((inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>()) != null)
            {
                inGameSceneMain.EnemyCacheSystem.GenerateCache(enemyFiles[i].filePath, go, enemyFiles[i].cacheCount, transform);
            }
            else
            {
                Debug.LogError("Enemy prepare Error! inGameSceneMain is null");
            }
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Prepare();
    }
}
