using MLAPI.Serialization;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct SquadronMemberStruct : INetworkSerializable
{
    public int Index;
    public int EnemyId;
    public float GeneratePointX;
    public float GeneratePointY;
    public float AppearPointX;
    public float AppearPointY;
    public float DisappearPointX;
    public float DisappearPointY;

    public void NetworkSerialize(NetworkSerializer serializer)
    {
        serializer.Serialize(ref Index);
        serializer.Serialize(ref EnemyId);
        serializer.Serialize(ref GeneratePointX);
        serializer.Serialize(ref GeneratePointY);
        serializer.Serialize(ref AppearPointX);
        serializer.Serialize(ref AppearPointY);
        serializer.Serialize(ref DisappearPointX);
        serializer.Serialize(ref DisappearPointY);
    }
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

    public int GetCount()
    {
        return tableDatas.Count;
    }

    protected override void AddData(SquadronMemberStruct data)
    {
        base.AddData(data);
        tableDatas.Add(data);
    }
}
