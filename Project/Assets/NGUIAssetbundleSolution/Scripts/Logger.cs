#define TRACE_ON
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
class Logger
{
    [Conditional("TRACE_ON")]
    public static void Log(string str)
    {
        Debug.Log(str);
    }
    [Conditional("TRACE_ON")]
    public static void Log(params string[] list)
    {
        string str = string.Empty;
        for (int i = 0; i < list.Length; i++)
        {
            str += list[i] + " ";
        }
        Debug.Log(str);
    }


    public static string GetFullName(GameObject inObject)
    {
        if (!inObject)
            return string.Empty;
        if (inObject.transform.parent)
            return Logger.GetFullName(inObject.transform.parent.gameObject) + "/" + inObject.name;
        return inObject.name;
    }
}
