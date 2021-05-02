using UnityEngine;

public class BasePanel : MonoBehaviour
{
    public virtual void InitializePanel()
    {
        PanelManager.RegistPanel(GetType(), this);
    }

    public virtual void DestroyPanel()
    {
        PanelManager.UnregistPanel(GetType());
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        InitializePanel();
    }

    private void OnDestroy()
    {
        DestroyPanel();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Close"))
        {
            Close();
        }
    }
}
