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
    readonly Dictionary<string, Queue<GameObject>> caches = new Dictionary<string, Queue<GameObject>>();

    public void GenerateCache(string filePath, GameObject gameObject, int cacheCount)
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
                GameObject go = Object.Instantiate(gameObject);
                go.SetActive(false);
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

        caches[filePath].Enqueue(gameObject);
        return true;
    }
}
