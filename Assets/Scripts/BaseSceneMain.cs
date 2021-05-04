using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSceneMain : MonoBehaviour
{
    /// <summary>
    /// 외부에서 초기화 호출
    /// </summary>
    public virtual void Initialize()
    {

    }

    protected virtual void OnAwake()
    {

    }

    protected virtual void OnStart()
    {

    }

    protected virtual void UpdateScene()
    {

    }

    protected virtual void Terminate()
    {

    }

    // Start is called before the first frame update
    private void Start()
    {
        OnStart();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateScene();
    }

    private void OnDestroy()
    {
        Terminate();
    }
}
