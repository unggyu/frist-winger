using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNameConstants
{
    public const string TitleScene = "TitleScene";
    public const string LoadingScene = "LoadingScene";
    public const string InGame = "InGame";
}

public class SceneController : MonoBehaviour
{
    private static SceneController instance = null;

    public static SceneController Instance
    {
        get
        {
            if (instance == null)
            {
                // ���� ��� �� Ŭ������� ���� �̸��� ���� ������Ʈ�� ���� ����ġ
                GameObject go = GameObject.Find("SceneController");
                if (go == null)
                {
                    go = new GameObject("SceneController");

                    SceneController controller = go.AddComponent<SceneController>();
                    return controller;
                }
                else
                {
                    instance = go.GetComponent<SceneController>();
                }
            }

            return instance;
        }
    }

    /// <summary>
    /// ���� Scene�� Unload �ϰ� �ε�
    /// </summary>
    /// <param name="sceneName">�ε��� Scene�� �̸�</param>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName, LoadSceneMode.Single));
    }

    /// <summary>
    /// ���� Scene�� Unload ���� �ε�
    /// </summary>
    /// <param name="sceneName">�ε��� Scene�� �̸�</param>
    public void LoadSceneAdditive(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName, LoadSceneMode.Additive));
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Can't have two instance of singletone.");
            DestroyImmediate(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this); // �ı����� �ʰ� ����
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Scene ��ȭ�� ���� �̺�Ʈ �޼ҵ带 ����
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnActiveSceneChanged(Scene scene0, Scene scene1)
    {
        Debug.Log("OnActiveSceneChanged is called! scene0 = " + scene0.name + ", scene1 = " + scene1.name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        Debug.Log("OnSceneLoaded is called! scene = " + scene.name + ", loadSceneMode = " + loadSceneMode.ToString());
    }

    private void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("OnSceneUnloaded is called! scene = " + scene.name);
    }

    private IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);

        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        Debug.Log("LoadSceneAsync is completed");
    }
}
