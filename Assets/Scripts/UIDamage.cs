using UnityEngine;
using UnityEngine.UI;

public class UIDamage : MonoBehaviour
{
    private enum DamageState : int
    {
        None = 0,
        SizeUp,
        Display,
        FadeOut
    }

    private const float sizeUpDuration = 0.1f;
    private const float displayDuration = 0.5f;
    private const float fadeOutDuration = 0.2f;

    private float displayStartTime;
    private float fadeOutStartTime;

    [SerializeField] private DamageState damageState = DamageState.None;
    [SerializeField] private Text damageText = null;
    [SerializeField] private Vector3 currentVelocity = Vector3.zero;

    public string FilePath { get; set; }

    public void ShowDamage(int damage, Color textColor)
    {
        damageText.text = damage.ToString();
        damageText.color = textColor;
        Reset();
        damageState = DamageState.SizeUp;
    }

    private void Update()
    {
        UpdateDamage();
    }

    private void Reset()
    {
        transform.localScale = Vector3.zero;
        Color newColor = damageText.color;
        newColor.a = 1.0f;
        damageText.color = newColor;
    }

    private void UpdateDamage()
    {
        if (damageState == DamageState.None)
        {
            return;
        }

        switch (damageState)
        {
            case DamageState.SizeUp:
                transform.localScale = Vector3.SmoothDamp(transform.localScale, Vector3.one, ref currentVelocity, sizeUpDuration);

                if (transform.localScale == Vector3.one)
                {
                    damageState = DamageState.Display;
                    displayStartTime = Time.time;
                }
                break;
            case DamageState.Display:
                if (Time.time - displayStartTime > displayDuration)
                {
                    damageState = DamageState.FadeOut;
                    fadeOutStartTime = Time.time;
                }
                break;
            case DamageState.FadeOut:
                Color newColor = damageText.color;
                newColor.a = Mathf.Lerp(1, 0, (Time.time - fadeOutStartTime) / fadeOutDuration);
                damageText.color = newColor;

                if (newColor.a == 0)
                {
                    damageState = DamageState.None;
                    SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().DamageManager.Remove(this);
                }
                break;
        }
    }
}
