using MLAPI;
using MLAPI.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneMain : BaseSceneMain
{
    private const float nextSceneInterval = 3.0f;
    private const float textUpdateInterval = 0.15f;
    private const string loadingTextValue = "Loading...";

    private int textIndex = 0;
    private float lastUpdateTime;
    private float sceneStartTime;
    private bool nextSceneCall = false;

    [SerializeField] Text loadingText = null;

    protected override void OnStart()
    {
        base.OnStart();
        sceneStartTime = Time.time;
    }

    protected override void UpdateScene()
    {
        base.UpdateScene();

        float currentTime = Time.time;
        if (currentTime - lastUpdateTime > textUpdateInterval)
        {
            loadingText.text = loadingTextValue.Substring(0, textIndex + 1);

            textIndex++;
            if (textIndex >= loadingTextValue.Length)
            {
                textIndex = 0;
            }

            lastUpdateTime = currentTime;
        }

        if (currentTime - sceneStartTime > nextSceneInterval)
        {
            if (!nextSceneCall)
            {
                GotoNextScene();
            }
        }
    }

    private void GotoNextScene()
    {
        // SceneController.Instance.LoadScene(SceneNameConstants.InGame);
        NetworkManager.Singleton.StartHost();
        NetworkSceneManager.SwitchScene(SceneNameConstants.InGame);
        nextSceneCall = true;
    }
}
