using MLAPI;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public const int PlayerBulletIndex = 0;
    public const int EnemyBulletIndex = 1;

    private readonly Dictionary<string, GameObject> fileCache = new Dictionary<string, GameObject>();

    [SerializeField] PrefabCacheData[] bulletFiles = null;

    public Bullet Generate(int index)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return null;
        }

        if (index < 0 || index >= bulletFiles.Length)
        {
            Debug.LogError("Generate error! out of range! index = " + index);
            return null;
        }

        string filePath = bulletFiles[index].filePath;
        GameObject go = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletCacheSystem.Archive(filePath);
        Bullet bullet = go.GetComponent<Bullet>();

        return bullet;
    }

    public GameObject Load(string resourcePath)
    {
        Debug.Log("Bullet load. resourcePath = " + resourcePath);
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
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        for (int i = 0; i < bulletFiles.Length; i++)
        {
            GameObject go = Load(bulletFiles[i].filePath);
            InGameSceneMain inGameSceneMain;
            if ((inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>()) != null)
            {
                inGameSceneMain.BulletCacheSystem.GenerateCache(bulletFiles[i].filePath, go, bulletFiles[i].cacheCount, transform);
            }
            else
            {
                Debug.LogError("Bullet prepare Error! inGameSceneMain is null");
            }
        }
    }

    public bool Remove(Bullet bullet)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return false;
        }

        return SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletCacheSystem.Restore(bullet.FilePath, bullet.gameObject);
    }

    private void Start()
    {
        // Host에서만 Prepare를 해야함
        // Prepare();
    }
}
