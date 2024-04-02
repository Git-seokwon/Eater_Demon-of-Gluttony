using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ※ Attribute
// → 변수의 Data를 Binding 하거나 어떤 Flag를 설정하는 용도로 많이 쓰이기 때문에 
//    대부분 내용이 간단하다
public class UnderlineTitleAttribute : PropertyAttribute // Custom Property Attribute
                                                         // → PropertyDrawer 클래스와 연결되어 해당 속성이 있는 스크립트 변수가 인스펙터창에
                                                         //    표시되는 방식을 제어할 수 있다. 
{
    // Title Text 띄워줄 공간 Property
    public string Title {  get; private set; }

    // 윗쪽 GUI와 띄워줄 공간 Property
    public int Space { get; private set; }

    // 생성자
    public UnderlineTitleAttribute(string title, int space = 12)
    {
        Title = title;
        Space = space;
    }
}
