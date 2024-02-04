using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1. 추상 클래스
// 2. where 키워드
// => 원래 <T>만 사용하면 참조 형식 뿐만 아니라 값형식도 올 수 있지만, where 키워드로 MonoBehaviour를 상속받는 class만 올 수 있다. 
public abstract class SingletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour // 씬 이동은 없게 할 거지만 혹시나 몰라 싱글톤으로 만듬
{
    private static T instance;

    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    // 싱글톤 구현
    protected virtual void Awake()
    {
        if (instance == null) 
        {
            // as 키워드를 사용하여 this -> T 자료형으로 캐스팅한다. 
            instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
