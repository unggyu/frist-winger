using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public const int EnemyDamageIndex = 0;
    public const int PlayerDamageIndex = 0;

    private readonly Dictionary<string, GameObject> fileCache = new Dictionary<string, GameObject>();

    [SerializeField] private Transform canvasTransform = null;
    [SerializeField] private PrefabCacheData[] files = null;

    public void Prepare()
    {
        for (int i = 0; i < files.Length; i++)
        {
            GameObject go = Load(files[i].filePath);
            InGameSceneMain inGameSceneMain;
            if ((inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>()) != null)
            {
                inGameSceneMain.DamageCacheSystem.GenerateCache(files[i].filePath, go, files[i].cacheCount, canvasTransform);
            }
        }
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

    public UIDamage Generate(int index, Vector3 position, int damageValue, Color textColor)
    {
        if (index < 0 || index >= files.Length)
        {
            Debug.LogError("Generate error! out of range! index = " + index);
            return null;
        }

        string filePath = files[index].filePath;
        GameObject go = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().DamageCacheSystem.Archive(filePath);
        go.transform.position = Camera.main.WorldToScreenPoint(position);
        Debug.Log("Damage generate to world position = " + go.transform.position + ", position = " + position);

        UIDamage damage = go.GetComponent<UIDamage>();
        damage.FilePath = filePath;
        damage.ShowDamage(damageValue, textColor);

        return damage;
    }

    public bool Remove(UIDamage damage)
    {
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().DamageCacheSystem.Restore(damage.FilePath, damage.gameObject);
        return true;
    }

    // Start is called before the first frame update
    private void Start()
    {
        Prepare();
    }
}
