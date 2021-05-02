using UnityEngine;
using UnityEngine.UI;

public class Gage : MonoBehaviour
{
    [SerializeField] Slider slider = null;

    public void SetHp(float currentValue, float maxValue)
    {
        if (currentValue > maxValue)
        {
            currentValue = maxValue;
        }

        slider.value = currentValue / maxValue;
    }
}
