using UnityEngine;
using UnityEngine.UI;

public class PlayerStatePanel : BasePanel
{
    [SerializeField]
    Text scoreText = null;

    [SerializeField]
    Gage hpGage = null;

    public void SetScore(int value)
    {
        Debug.Log("SetScore value = " + value);

        scoreText.text = value.ToString();
    }

    public void SetHp(float currentValue, float maxValue)
    {
        hpGage.SetHp(currentValue, maxValue);
    }
}
