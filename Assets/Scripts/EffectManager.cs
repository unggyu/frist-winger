using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public const int BulletDisappearFxIndex = 0;
    public const int ActorDeadFxIndex = 1;

    private readonly Dictionary<string, GameObject> fileCache = new Dictionary<string, GameObject>();

    [SerializeField] private PrefabCacheData[] effectFiles = null;

    public GameObject GenerateEffect(int index, Vector3 position)
    {
        if (index < 0 || index >= effectFiles.Length)
        {
            Debug.LogError("GenerateEffect error! out of range! index = " + index);
            return null;
        }

        string filePath = effectFiles[index].filePath;
        GameObject go = SystemManager
            .Instance
            .GetCurrentSceneMain<InGameSceneMain>()
            .EffectCacheSystem
            .Archive(filePath);

        go.transform.position = position;

        AutoCachableEffect effect = go.GetComponent<AutoCachableEffect>();
        effect.FilePath = filePath;

        return go;
    }

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
        for (int i = 0; i < effectFiles.Length; i++)
        {
            GameObject go = Load(effectFiles[i].filePath);
            InGameSceneMain inGameSceneMain;
            if ((inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>()) != null)
            {
                inGameSceneMain.EffectCacheSystem.GenerateCache(effectFiles[i].filePath, go, effectFiles[i].cacheCount, transform);
            }
            else
            {
                Debug.LogError("Effect prepare Error! inGameSceneMain is null");
            }
        }
    }

    public bool RemoveEffect(AutoCachableEffect effect)
    {
        SystemManager
            .Instance
            .GetCurrentSceneMain<InGameSceneMain>()
            .EffectCacheSystem
            .Restore(effect.FilePath, effect.gameObject);

        return true;
    }

    private void Start()
    {
        Prepare();
    }
}
