public class TitleSceneMain : BaseSceneMain
{
    public void OnStartButton()
    {
        PanelManager.GetPanel(typeof(NetworkConfigPanel)).Show();
    }

    public void GoToNextScene()
    {
        SceneController.Instance.LoadScene(SceneNameConstants.LoadingScene);
    }
}
