using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct EnemyStruct
{
    public int Index;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MarshalTableConstant.charBufferSize)]
    public string FilePath;
    public int MaxHp;
    public int Damage;
    public int CrashDamage;
    public int BulletSpeed;
    public int FireRemainCount;
    public int GamePoint;
}

public class EnemyTable : TableLoader<EnemyStruct>
{
    private readonly Dictionary<int, EnemyStruct> tableDatas = new Dictionary<int, EnemyStruct>();

    public EnemyStruct GetEnemy(int index)
    {
        if (!tableDatas.ContainsKey(index))
        {
            Debug.LogError("GetEnemy Error! index = " + index);
            return default;
        }

        return tableDatas[index];
    }

    protected override void AddData(EnemyStruct data)
    {
        base.AddData(data);
        tableDatas.Add(data.Index, data);
    }

    private void Start()
    {
        Load();
    }
}
