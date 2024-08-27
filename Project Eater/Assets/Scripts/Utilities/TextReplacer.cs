using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �� TextReplacer
// �� Text�� �ִ� Mark�� �� �� ���� Replace �ϱ� ���� Utility Class
// �� Mark : $[]
public static class TextReplacer 
{
    // ���忡 �ִ� Ư�� Keyword�� Replace�ϴ� �Լ�
    // �� textsByKeyword�� Key�� Replace ��� keyword, Value�� Replace�� ���� Value
    public static string Replace(string text, IReadOnlyDictionary<string, string> textByKeyword)
    {
        if (textByKeyword != null)
        {
            foreach (KeyValuePair<string, string> pair in textByKeyword)
                text = text.Replace($"$[{pair.Key}]", pair.Value);
        }

        return text;
    }

    // ���ξ��� prefixKeyword�� �ִ� ������ Replace
    // �� prefixKeyword�� Keyword�� '.'���� ����
    public static string Replace(string text, string prefixKeyword, IReadOnlyDictionary<string, string> textByKeyword)
    {
        if (textByKeyword != null)
        {
            foreach (KeyValuePair<string, string> pair in textByKeyword)
                text = text.Replace($"$[{prefixKeyword}.{pair.Key}]", pair.Value);
        }

        return text;
    }

    // ���̾��� suffixKeyword�� �ִ� ������ Replace
    // �� suffixKeyword�� Keyword�� '.'���� ����
    public static string Replace(string text, IReadOnlyDictionary<string, string> textByKeyword, string suffixKeyword)
    {
        if (textByKeyword != null)
        {
            foreach (KeyValuePair<string, string> pair in textByKeyword)
                text = text.Replace($"$[{pair.Key}.{suffixKeyword}]", pair.Value);
        }

        return text;
    }

    // ���ξ��� prefixKeyword�� ���̾��� suffixKeyword �� �� �ִ� ������ Replace
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
