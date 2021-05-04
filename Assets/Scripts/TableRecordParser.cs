using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class MarshalTableConstant
{
    public const int charBufferSize = 256;
}

public class TableRecordParser<TMarshalStruct>
{
    /// <summary>
    /// 마샬링을 통한 byte 배열의 T형 구조체 변환
    /// </summary>
    /// <typeparam name="T">마샬링에 적합하게 정의된 구조체 타입</typeparam>
    /// <param name="bytes">마샬링할 데이터가 저장된 배열</param>
    /// <returns>변환된 T형 구조체</returns>
    public static T MakeStructFromBytes<T>(byte[] bytes)
    {
        int size = Marshal.SizeOf(typeof(T));
        IntPtr ptr = Marshal.AllocHGlobal(size); // 마샬 메모리 할당

        Marshal.Copy(bytes, 0, ptr, size); // 복사

        T tStruct = (T)Marshal.PtrToStructure(ptr, typeof(T)); // 메모리로부터 T형 구조체로 변환
        Marshal.FreeHGlobal(ptr); // 할당된 메모리 해제
        return tStruct; // 변환된 값 반환
    }

    public TMarshalStruct ParseRecordLine(string line)
    {
        // TMarshalStruct 크기에 맞춰서 Byte 배열 할당
        Type type = typeof(TMarshalStruct);
        int structSize = Marshal.SizeOf(type);
        byte[] structBytes = new byte[structSize];
        int structBytesIndex = 0;

        // line 문자열을 spliter로 자름
        const char spliter = ',';
        string[] fieldDataList = line.Split(spliter);
        // 타입을 보고 바이너리에 파싱하여 삽입
        Type dataType;
        string splited;
        byte[] fieldByte;
        // byte[] keyBytes;

        FieldInfo[] fieldInfos = type.GetFields();
        for (int i = 0; i < fieldInfos.Length; i++)
        {
            dataType = fieldInfos[i].FieldType;
            splited = fieldDataList[i];

            fieldByte = new byte[4];
            MakeBytesByFieldType(out fieldByte, dataType, splited);

            Buffer.BlockCopy(fieldByte, 0, structBytes, structBytesIndex, fieldByte.Length);
            structBytesIndex += fieldByte.Length;
        }

        TMarshalStruct tStruct = MakeStructFromBytes<TMarshalStruct>(structBytes);
        return tStruct;
    }

    /// <summary>
    /// 문자열 splite을 주어진 dataType에 맞게 fieldByte 배열에 반환해서 반환
    /// </summary>
    /// <param name="fieldByte">결과 값을 받을 배열</param>
    /// <param name="dataType">splite을 변환할 때 사용될 자료형</param>
    /// <param name="splite">변환할 값이 있는 문자열</param>
    protected void MakeBytesByFieldType(out byte[] fieldByte, Type dataType, string splite)
    {
        fieldByte = new byte[1];

        if (typeof(int) == dataType)
        {
            fieldByte = BitConverter.GetBytes(int.Parse(splite));
        }
        else if (typeof(float) == dataType)
        {
            fieldByte = BitConverter.GetBytes(float.Parse(splite));
        }
        else if (typeof(bool) == dataType)
        {
            bool value = bool.Parse(splite);
            int temp = value ? 1 : 0;

            fieldByte = BitConverter.GetBytes(temp);
        }
        else if (typeof(string) == dataType)
        {
            fieldByte = new byte[MarshalTableConstant.charBufferSize];
            byte[] byteArr = Encoding.UTF8.GetBytes(splite);
            Buffer.BlockCopy(byteArr, 0, fieldByte, 0, byteArr.Length);
        }
    }
}
