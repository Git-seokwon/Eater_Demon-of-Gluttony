using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ※ TextReplacer
// → Text에 있는 Mark를 좀 더 쉽게 Replace 하기 위한 Utility Class
// ★ Mark : $[]
public static class TextReplacer 
{
    // 문장에 있는 특정 Keyword를 Replace하는 함수
    // → textsByKeyword의 Key는 Replace 대상 keyword, Value는 Replace해 넣을 Value
    public static string Replace(string text, IReadOnlyDictionary<string, string> textByKeyword)
    {
        if (textByKeyword != null)
        {
            foreach (KeyValuePair<string, string> pair in textByKeyword)
                text = text.Replace($"$[{pair.Key}]", pair.Value);
        }

        return text;
    }

    // 접두어인 prefixKeyword가 있는 버전의 Replace
    // → prefixKeyword와 Keyword는 '.'으로 구분
    public static string Replace(string text, string prefixKeyword, IReadOnlyDictionary<string, string> textByKeyword)
    {
        if (textByKeyword != null)
        {
            foreach (KeyValuePair<string, string> pair in textByKeyword)
                text = text.Replace($"$[{prefixKeyword}.{pair.Key}]", pair.Value);
        }

        return text;
    }

    // 접미어인 suffixKeyword가 있는 버전의 Replace
    // → suffixKeyword와 Keyword는 '.'으로 구분
    public static string Replace(string text, IReadOnlyDictionary<string, string> textByKeyword, string suffixKeyword)
    {
        if (textByKeyword != null)
        {
            foreach (KeyValuePair<string, string> pair in textByKeyword)
                text = text.Replace($"$[{pair.Key}.{suffixKeyword}]", pair.Value);
        }

        return text;
    }

    // 접두어인 prefixKeyword와 접미어인 suffixKeyword 둘 다 있는 버전의 Replace
    public static string Replace(string text, string prefixKeyword, IReadOnlyDictionary<string, string> textByKeyword, string suffixKeyword)
    {
        if (textByKeyword != null)
        {
            foreach (KeyValuePair<string, string> pair in textByKeyword)
                text = text.Replace($"$[{prefixKeyword}.{pair.Key}.{suffixKeyword}]", pair.Value);
        }

        return text;
    }
}
