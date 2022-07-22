using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public static class StringUtils
{
    public static bool Contains(this string source, string toCheck, StringComparison comp)
    {
        return source.IndexOf(toCheck, comp) >= 0;
    }
    /// <summary>
    /// 首字母小写写
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string FirstCharToLower(this string input)
    {
        if (String.IsNullOrEmpty(input))
            return input;
        string str = input.First().ToString().ToLower() + input.Substring(1);
        return str;
    }
 
    /// <summary>
    /// 首字母大写
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string FirstCharToUpper(this string input)
    {
        if (String.IsNullOrEmpty(input))
            return input;
        string str = input.First().ToString().ToUpper() + input.Substring(1);
        return str;
    }
}


