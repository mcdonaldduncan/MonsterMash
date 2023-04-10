using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeLookup
{
    public static Dictionary<(ElementType, ElementType), float> Lookup;

    // This is temporary and all types do even damage, will change to be loaded via file
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
        return Lookup[(attackType, defensetype)];
    }

}


public enum ElementType
{
    GRASS,
    WATER,
    FIRE,
    SHADOW
}