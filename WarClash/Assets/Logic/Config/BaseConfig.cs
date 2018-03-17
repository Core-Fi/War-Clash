using System;
using System.Text;

public abstract class BaseConfig
{
    public int ToInt(byte[] bytes, ref int startIndex)
    {

        if (bytes[startIndex] == 0)
        {
            startIndex++;
            return 0;
        }
        startIndex++;
        var v =  BitConverter.ToInt32(bytes, startIndex);
        startIndex += 4;
        return v;
    }
    public float ToFloat(byte[] bytes, ref int startIndex)
    {
        if (bytes[startIndex] == 0)
        {
            startIndex++;
            return 0;
        }
        startIndex++;
        var v = BitConverter.ToSingle(bytes, startIndex);
        startIndex += 4;
        return v;
    }
    public string ToString(byte[] bytes, ref int startIndex)
    {
        if (bytes[startIndex] == 0)
        {
            startIndex++;
            return "";
        }
        startIndex++;
        var strLen = BitConverter.ToInt32(bytes, startIndex);
        startIndex += 4;
        var str = Encoding.UTF8.GetString(bytes, startIndex, strLen);
        startIndex += strLen;
        return str;

    }
    public int[] ToIntArray(byte[] bytes, ref int startIndex)
    {
        if (bytes[startIndex] == 0)
        {
            startIndex++;
            return null;
        }
        startIndex++;
        var strLen = BitConverter.ToInt32(bytes, startIndex);
        startIndex += 4;
        int[] array = new int[strLen];
        for (int i = 0; i < strLen; i++)
        {
            var v = BitConverter.ToInt32(bytes, startIndex);
            array[i] = v;
            startIndex += 4;
        }
        return array;
    }
    public float[] ToFloatArray(byte[] bytes, ref int startIndex)
    {
        if (bytes[startIndex] == 0)
        {
            startIndex++;
            return null;
        }
        startIndex++;
        var strLen = BitConverter.ToInt32(bytes, startIndex);
        startIndex += 4;
        float[] array = new float[strLen];
        for (int i = 0; i < strLen; i++)
        {
            var v = BitConverter.ToSingle(bytes, startIndex);
            array[i] = v;
            startIndex += 4;
        }
        return array;
    }
}
