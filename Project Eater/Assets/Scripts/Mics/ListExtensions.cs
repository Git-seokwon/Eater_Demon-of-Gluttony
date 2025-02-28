using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static T AddAndReturn<T>(this List<T> list, T item)
    {
        list.Add(item);
        return item;
    }
}
