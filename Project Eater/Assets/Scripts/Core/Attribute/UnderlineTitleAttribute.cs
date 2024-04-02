using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �� Attribute
// �� ������ Data�� Binding �ϰų� � Flag�� �����ϴ� �뵵�� ���� ���̱� ������ 
//    ��κ� ������ �����ϴ�
public class UnderlineTitleAttribute : PropertyAttribute // Custom Property Attribute
                                                         // �� PropertyDrawer Ŭ������ ����Ǿ� �ش� �Ӽ��� �ִ� ��ũ��Ʈ ������ �ν�����â��
                                                         //    ǥ�õǴ� ����� ������ �� �ִ�. 
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
