using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    public const string enemyPath = "Prefabs/Enemy";
    readonly Dictionary<string, GameObject> enemyFileCache = new Dictionary<string, GameObject>();

    public GameObject Load(string resourcePath)
    {
        Debug.Log("Enemy load. resourcePath = " + resourcePath);

        GameObject go;

        if (enemyFileCache.ContainsKey(resourcePath))
        {
            go = enemyFileCache[resourcePath];
        }
        else
        {
            // 캐시에 없으므로 로드
            go = Resources.Load<GameObject>(resourcePath);
            if (!go)
            {
                Debug.LogError("Enemy load error! path = " + resourcePath);
                return null;
            }
            // 로드 후 캐시에 적재
            enemyFileCache.Add(resourcePath, go);
        }

        return go;
    }
}
