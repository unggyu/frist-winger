using System.IO;
using UnityEngine;

public class TableLoader<TMarshalStruct> : MonoBehaviour
{
    TableRecordParser<TMarshalStruct> tableRecordParser = new TableRecordParser<TMarshalStruct>();

    [SerializeField] protected string filePath;

    public bool Load()
    {
        TextAsset textAsset = Resources.Load<TextAsset>(filePath);
        if (textAsset == null)
        {
            Debug.LogError("Load Failed! filePath = " + textAsset);
            return false;
        }

        ParseTable(textAsset.text);

        return true;
    }

    protected virtual void AddData(TMarshalStruct data)
    {

    }

    private void ParseTable(string text)
    {
        StringReader reader = new StringReader(text);

        string line;
        bool fieldRead = false;

        while ((line = reader.ReadLine()) != null) // 파일 끝날 때까지 계속 레코드 파싱
        {
            if (!fieldRead)
            {
                fieldRead = true;
                continue;
            }

            TMarshalStruct data = tableRecordParser.ParseRecordLine(line);
            AddData(data);
        }
    }
}
