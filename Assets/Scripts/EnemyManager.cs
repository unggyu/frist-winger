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
        string filePath = SystemManager.Instance.EnemyTable.GetEnemy(data.EnemyId).FilePath;
        GameObject go = SystemManager
            .Instance
            .GetCurrentSceneMain<InGameSceneMain>()
            .EnemyCacheSystem
            .Archive(filePath);

        go.transform.position = new Vector3(data.GeneratePointX, data.GeneratePointY, 0);

        Enemy enemy = go.GetComponent<Enemy>();
        enemy.FilePath = filePath;
        enemy.Reset(data);

        enemies.Add(enemy);
        return true;
    }

    public bool RemoveEnemy(Enemy enemy)
    {
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
        for (int i = 0; i < enemyFiles.Length; i++)
        {
            GameObject go = enemyFactory.Load(enemyFiles[i].filePath);
            SystemManager
                .Instance
                .GetCurrentSceneMain<InGameSceneMain>()
                .EnemyCacheSystem
                .GenerateCache(enemyFiles[i].filePath, go, enemyFiles[i].cacheCount);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        Prepare();
    }
}
