using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct SquadronScheduleDataStruct
{
    public int Index;
    public float GenerateTime;
    public int SquadronId;
}

public class SquadronScheduleTable : TableLoader<SquadronScheduleDataStruct>
{
    private readonly List<SquadronScheduleDataStruct> tableDatas = new List<SquadronScheduleDataStruct>();

    protected override void AddData(SquadronScheduleDataStruct data)
    {
        base.AddData(data);
        tableDatas.Add(data);
    }

    public SquadronScheduleDataStruct GetScheduleData(int index)
    {
        if (index < 0 || index >= tableDatas.Count)
        {
            Debug.LogError("GetScheduleData Error! index = " + index);
            return default;
        }

        return tableDatas[index];
    }

    /// <summary>
    /// 데이터 개수 제공
    /// </summary>
    /// <returns></returns>
    public int GetDataCount()
    {
        return tableDatas.Count;
    }
}
