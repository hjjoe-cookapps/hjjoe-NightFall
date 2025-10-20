using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using System.ComponentModel;

public static class Enums
{
    public static string ToDescription(this Enum source) // from enum get description as string if not exists get name as string
    {
        FieldInfo info = source.GetType().GetField(source.ToString());

        var attribute = (DescriptionAttribute) info.GetCustomAttribute(typeof(DescriptionAttribute), false);

        if (attribute != null)
        {
            return attribute.Description;
        }

        return source.ToString();
    }

    public static T GetRandomEnum<T>() where T : Enum
    {
        Array values = Enum.GetValues(typeof(T));
        return (T) values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }
    
    public static T GetRandomEnum<T>(this T _) where T : Enum
    {
        Array values = Enum.GetValues(typeof(T));
        return (T) values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }
}
