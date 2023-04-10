using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeLookup
{
    public static Dictionary<ElementType[][], float> Lookup;


    public static void Init()
    {
        Lookup = new Dictionary<ElementType[][], float>();

        foreach (var typeA in Enum.GetNames(typeof(ElementType))
        {

        }
    }

}


public enum ElementType
{
    GRASS,
    WATER,
    FIRE,
    SHADOW
}