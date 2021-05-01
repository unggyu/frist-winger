using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class AutoCachableEffect : MonoBehaviour
{
    public string FilePath { get; set; }

    private void OnEnable()
    {
        StartCoroutine(CheckIfAfter());
    }

    IEnumerator CheckIfAfter()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (!GetComponent<ParticleSystem>().IsAlive(true))
            {
                SystemManager.Instance.EffectManager.RemoveEffect(this);
                break;
            }
        }
    }
}
