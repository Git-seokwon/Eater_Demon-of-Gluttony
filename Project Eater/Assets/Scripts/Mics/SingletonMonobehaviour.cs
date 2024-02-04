using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1. �߻� Ŭ����
// 2. where Ű����
// => ���� <T>�� ����ϸ� ���� ���� �Ӹ� �ƴ϶� �����ĵ� �� �� ������, where Ű����� MonoBehaviour�� ��ӹ޴� class�� �� �� �ִ�. 
public abstract class SingletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour // �� �̵��� ���� �� ������ Ȥ�ó� ���� �̱������� ����
{
    private static T instance;

    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    // �̱��� ����
    protected virtual void Awake()
    {
        if (instance == null) 
        {
            // as Ű���带 ����Ͽ� this -> T �ڷ������� ĳ�����Ѵ�. 
            instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
