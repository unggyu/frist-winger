using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneMain : BaseSceneMain
{
    private const float textUpdateInterval = 0.15f;
    private const string loadingTextValue = "Loading...";

    private int textIndex = 0;
    private float lastUpdateTime;

    [SerializeField] Text loadingText = null;

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
    }
}
