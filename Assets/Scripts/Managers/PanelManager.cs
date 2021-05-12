using System;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    private static readonly Dictionary<Type, BasePanel> panels = new Dictionary<Type, BasePanel>();

    public static bool RegistPanel(Type panelClassType, BasePanel basePanel)
    {
        if (panels.ContainsKey(panelClassType))
        {
            Debug.LogError("RegistPanel Error! Already exists Type! panelClassType = " + panelClassType.ToString());
            return false;
        }

        Debug.Log("RegisterPanel is called! Type = " + panelClassType.ToString() + ", basePanel = " + basePanel.name);

        panels.Add(panelClassType, basePanel);
        return true;
    }

    public static bool UnregistPanel(Type panelClassType)
    {
        if (!panels.ContainsKey(panelClassType))
        {
            Debug.LogError("UnregistPanel Error! Can't find type! panelClassType = " + panelClassType.ToString());
            return false;
        }

        panels.Remove(panelClassType);
        return true;
    }

    public static BasePanel GetPanel(Type panelClassType)
    {
        if (!panels.ContainsKey(panelClassType))
        {
            Debug.LogError("GetPanel Error! Can't find type! panelClassType = " + panelClassType.ToString());
            return null;
        }

        return panels[panelClassType];
    }
}
