using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeLookup
{
    public static Dictionary<(ElementType, ElementType), float> Lookup;

    // ToDo Change to load via file
    public static void Init()
    {
        Lookup = new Dictionary<(ElementType, ElementType), float>();

        var types = Enum.GetValues(typeof(ElementType));

        foreach (ElementType typeA in types)
        {
            foreach (ElementType typeB in types)
            {
                Lookup.Add((typeA, typeB), 1);
            }
        }
    }

    public static float GetEfficacy(ElementType attackType, ElementType defensetype)
    {
        if (!Lookup.TryGetValue((attackType, defensetype), out var value))
        {
            Utility.LogError($"Type comparison not found for {attackType}, {defensetype}");
            return 0;
        }

        return value;
    }

}

