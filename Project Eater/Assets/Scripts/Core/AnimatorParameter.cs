using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimatorParameterType
{
    Bool,
    Trigger
}

[System.Serializable]
public struct AnimatorParameter // Animator에 있는 Parameter의 정보를 담고 있는 구조체 
{
    public AnimatorParameterType parameterType;
    public string parameterName;

    // 파라미터 hash 값 
    private int hash;

    // AnimatorParameter 구조체가 유효한지 여부 
    public bool isValid => !string.IsNullOrEmpty(parameterName);

    public int Hash
    {
        get
        {
            // parameterName에 해당하는 AnimatorHash 값을 구해서 hash 변수에 저장 
            // → Hash Property가 어딘가에서 최초로 불리게 될 때, hash 값이 Setting 된다. 
            if (hash == 0 && isValid)
                hash = Animator.StringToHash(parameterName);

            return hash;
        }
    }
}