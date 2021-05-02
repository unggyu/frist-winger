using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public const int PlayerBulletIndex = 0;
    public const int EnemyBulletIndex = 1;

    private readonly Dictionary<string, GameObject> fileCache = new Dictionary<string, GameObject>();

    [SerializeField] PrefabCacheData[] bulletFiles = null;

    public GameObject Load(string resourcePath)
    {
        GameObject go;

        if (fileCache.ContainsKey(resourcePath)) // 캐시 확인
        {
            go = fileCache[resourcePath];
        }
        else
        {
            // 캐시에 없으므로 로드
            go = Resources.Load<GameObject>(resourcePath);
            if (!go)
            {
                Debug.LogError("Load error! path = " + resourcePath);
                return null;
            }

            // 로드 후 캐시에 적재
            fileCache.Add(resourcePath, go);
        }

        return go;
    }

    public void Prepare()
    {
        for (int i = 0; i < bulletFiles.Length; i++)
        {
            GameObject go = Load(bulletFiles[i].filePath);
            SystemManager.Instance.BulletCacheSystem.GenerateCache(bulletFiles[i].filePath, go, bulletFiles[i].cacheCount);
        }
    }

    public Bullet Generate(int index)
    {
        if (index < 0 || index >= bulletFiles.Length)
        {
            Debug.LogError("Generate error! out of range! index = " + index);
            return null;
        }

        string filePath = bulletFiles[index].filePath;
        GameObject go = SystemManager.Instance.BulletCacheSystem.Archive(filePath);

        Bullet bullet = go.GetComponent<Bullet>();
        bullet.FilePath = filePath;

        return bullet;
    }

    public bool Remove(Bullet bullet)
    {
        return SystemManager.Instance.BulletCacheSystem.Restore(bullet.FilePath, bullet.gameObject);
    }

    private void Start()
    {
        Prepare();
    }
}
