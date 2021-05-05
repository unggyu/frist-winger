using UnityEngine;
using UnityEngine.UI;

public class PlayerStatePanel : BasePanel
{
    public static event System.EventHandler Initialized;

    [SerializeField] private Text scoreText = null;
    [SerializeField] private Gage hpGage = null;

    public void SetScore(int value)
    {
        Debug.Log("SetScore value = " + value);

        scoreText.text = value.ToString();
    }

    public void SetHp(float currentValue, float maxValue)
    {
        hpGage.SetHp(currentValue, maxValue);
    }

    public override void InitializePanel()
    {
        base.InitializePanel();

        Initialized?.Invoke(this, System.EventArgs.Empty);
    }
}
