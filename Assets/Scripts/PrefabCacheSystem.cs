using MLAPI;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PrefabCacheData
{
    public string filePath;
    public int cacheCount;
}

public class PrefabCacheSystem
{
    private readonly Dictionary<string, Queue<GameObject>> caches = new Dictionary<string, Queue<GameObject>>();

    public void GenerateCache(string filePath, GameObject gameObject, int cacheCount, Transform parentTransform = null)
    {
        if (caches.ContainsKey(filePath))
        {
            Debug.LogWarning("Already cache generated! filePath = " + filePath);
            return;
        }
        else
        {
            Queue<GameObject> queue = new Queue<GameObject>();
            for (int i = 0; i < cacheCount; i++)
            {
                GameObject go = Object.Instantiate(gameObject, parentTransform);
                NetworkObject networkObj = go.GetComponent<NetworkObject>();

                if (networkObj != null)
                {
                    Enemy enemy = go.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.FilePath = filePath;
                        networkObj.Spawn(); // active가 false 되기 전에 해야함
                        enemy.SetActiveClientRpc(false); // 클라이언트의 Enemy도 active가 false 되어야 함
                    }

                    Bullet bullet = go.GetComponent<Bullet>();
                    if (bullet != null)
                    {
                        bullet.FilePath = filePath;
                        networkObj.Spawn();
                        bullet.SetActive(false);
                    }
                }
                else
                {
                    go.SetActive(false);
                }

                queue.Enqueue(go);
            }

            caches.Add(filePath, queue);
        }
    }

    public GameObject Archive(string filePath)
    {
        if (!caches.ContainsKey(filePath))
        {
            Debug.LogError("Archive Error! no cache generated! filePath = " + filePath);
            return null;
        }

        if (caches[filePath].Count == 0)
        {
            Debug.LogWarning("Archive problem! not enough count");
            return null;
        }

        GameObject go = caches[filePath].Dequeue();
        go.SetActive(true);

        Enemy enemy = go.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.SetActiveClientRpc(true);
        }

        Bullet bullet = go.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.SetActive(true);
        }

        return go;
    }

    public bool Restore(string filePath, GameObject gameObject)
    {
        if (!caches.ContainsKey(filePath))
        {
            Debug.LogError("Restore Error! no cache generated! filePath = " + filePath);
            return false;
        }

        gameObject.SetActive(false);
        Enemy enemy = gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.SetActiveClientRpc(false);
        }

        caches[filePath].Enqueue(gameObject);
        return true;
    }

    public void Add(string filePath, GameObject gameObject)
    {
        Queue<GameObject> queue;
        if (caches.ContainsKey(filePath))
        {
            queue = caches[filePath];
        }
        else
        {
            queue = new Queue<GameObject>();
            caches.Add(filePath, queue);
        }

        queue.Enqueue(gameObject);
    }
}
