using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct SquadronMemberStruct
{
    public int Index;
    public int EnemyId;
    public int GeneratePointX;
    public int GeneratePointY;
    public int AppearPointX;
    public int AppearPointY;
    public int DisappearPointX;
    public int DisappearPointY;
}

public class SquadronTable : TableLoader<SquadronMemberStruct>
{
    private readonly List<SquadronMemberStruct> tableDatas = new List<SquadronMemberStruct> ();

    public SquadronMemberStruct GetSquadronMember(int index)
    {
        if (index < 0 || index >= tableDatas.Count)
        {
            Debug.LogError("GetSquadronMember Error! index = " + index);
            return default;
        }

        return tableDatas[index];
    }

    protected override void AddData(SquadronMemberStruct data)
    {
        base.AddData(data);
        tableDatas.Add(data);
    }
}
