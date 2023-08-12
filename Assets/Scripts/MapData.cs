using System;
using UnityEngine;
//I believe i was testing gernerics here
public interface IMappable
{
    public object GetValue();
    public int CompareTo(object obj);
    public float Map(object min, object max);
}

public struct MappableFloat : IMappable
{
    public float value;
    public object GetValue() => value;

    public int CompareTo(object obj) => value.CompareTo(obj);

    public float Map(object min, object max)
    {
        if (min is float Min && max is float Max)
        {
            return Mathf.InverseLerp(Min, Max, value);
        }
        else
        {
            throw new Exception("min or max value is not a float");
        }
    }
}

public struct MappableInt : IMappable
{
    public int value;
    public object GetValue() => value;

    public int CompareTo(object obj) => value.CompareTo(obj);

    public float Map(object min, object max)
    {
        if (min is int Min && max is int Max)
        {
            return Mathf.InverseLerp(Min, Max, value);
        }
        else
        {
            throw new Exception("min or max value is not a int");
        }
    }
}