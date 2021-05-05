using MLAPI;
using MLAPI.SceneManagement;
using MLAPI.Transports.UNET;
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
    [SerializeField] UNetTransport unetTransport = null;

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
        NetworkConnectionInfo info = SystemManager.Instance.ConnectionInfo;
        if (info.host)
        {
            Debug.Log("FW Start with host!");
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            Debug.Log("FW Start with client!");

            if (!string.IsNullOrWhiteSpace(info.ipAddress))
            {
                if (info.ipAddress.Equals("localhost"))
                {
                    info.ipAddress = "127.0.0.1";
                }
                unetTransport.ConnectAddress = info.ipAddress;
            }

            if (info.port != unetTransport.ConnectPort)
            {
                unetTransport.ConnectPort = info.port;
            }

            NetworkManager.Singleton.StartClient();
        }

        if (NetworkManager.Singleton.IsServer)
        {
            NetworkSceneManager.SwitchScene(SceneNameConstants.InGame);
        }
        nextSceneCall = true;
    }
}
