using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �� Attribute ��ũ��Ʈ : Attribute�� �����ϴ� Script
// �� ������ Data�� Binding �ϰų� � Flag�� �����ϴ� �뵵�� ���� ���̱� ������ 
//    ��κ� ������ �����ϴ� 
// �� Attribute�� ����� ���� PropertyAttribute�� ��ӹ��� 
// Ex) [UnderlineTitle("Action")]
//     Action
//     ------------
//     action 
public class UnderlineTitleAttribute : PropertyAttribute // Attribute�� ����� ���� PropertyAttribute�� ��ӹ��� 
{
    // Title Text ����� ���� Property
    public string Title {  get; private set; }

    // ���� GUI�� ����� ���� Property
    public int Space { get; private set; }

    // ������
    public UnderlineTitleAttribute(string title, int space = 12)
    {
        Title = title;
        Space = space;
    }
}
