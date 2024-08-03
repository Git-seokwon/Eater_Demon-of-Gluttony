using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimatorParameterType
{
    Bool,
    Trigger
}

[System.Serializable]
public struct AnimatorParameter // Animator�� �ִ� Parameter�� ������ ��� �ִ� ����ü 
{
    public AnimatorParameterType parameterType;
    public string parameterName;

    // �Ķ���� hash �� 
    private int hash;

    // AnimatorParameter ����ü�� ��ȿ���� ���� 
    public bool isValid => !string.IsNullOrEmpty(parameterName);

    public int Hash
    {
        get
        {
            // parameterName�� �ش��ϴ� AnimatorHash ���� ���ؼ� hash ������ ���� 
            // �� Hash Property�� ��򰡿��� ���ʷ� �Ҹ��� �� ��, hash ���� Setting �ȴ�. 
            if (hash == 0 && isValid)
                hash = Animator.StringToHash(parameterName);

            return hash;
        }
    }
}