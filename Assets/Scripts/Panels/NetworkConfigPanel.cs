using UnityEngine;
using UnityEngine.UI;

public class NetworkConfigPanel : BasePanel
{
    private const string defaultIpAddress = "localhost";
    private const string defaultPort = "7777";

    [SerializeField] private InputField ipAddressInputField;
    [SerializeField] private InputField portInputField;

    public override void InitializePanel()
    {
        base.InitializePanel();
        // IP와 Port 입력을 기본 값으로 셋팅
        ipAddressInputField.text = defaultIpAddress;
        portInputField.text = defaultPort;
        Close();
    }

    public void OnHostButton()
    {
        SystemManager.Instance.ConnectionInfo.host = true;
        TitleSceneMain sceneMain = SystemManager.Instance.GetCurrentSceneMain<TitleSceneMain>();
        sceneMain.GoToNextScene();
    }

    public void OnClientButton()
    {
        SystemManager.Instance.ConnectionInfo.host = false;

        if (!(string.IsNullOrWhiteSpace(ipAddressInputField.text) && ipAddressInputField.text.Equals(defaultIpAddress)))
        {
            SystemManager.Instance.ConnectionInfo.ipAddress = ipAddressInputField.text.Trim();
        }

        if (!(string.IsNullOrWhiteSpace(portInputField.text) && portInputField.text.Equals(defaultPort)))
        {
            if (int.TryParse(portInputField.text.Trim(), out int port))
            {
                SystemManager.Instance.ConnectionInfo.port = port;
            }
            else
            {
                Debug.LogError("OnClientButton error port = " + portInputField.text);
                return;
            }
        }

        TitleSceneMain sceneMain = SystemManager.Instance.GetCurrentSceneMain<TitleSceneMain>();
        sceneMain.GoToNextScene();
    }
}
