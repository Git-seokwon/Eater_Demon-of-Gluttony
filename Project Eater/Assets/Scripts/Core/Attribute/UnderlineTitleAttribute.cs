using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ※ Attribute 스크립트 : Attribute를 정의하는 Script
// → 변수의 Data를 Binding 하거나 어떤 Flag를 설정하는 용도로 많이 쓰이기 때문에 
//    대부분 내용이 간단하다 
// → Attribute를 만들기 위해 PropertyAttribute를 상속받음 
// Ex) [UnderlineTitle("Action")]
//     Action
//     ------------
//     action 
public class UnderlineTitleAttribute : PropertyAttribute // Attribute를 만들기 위해 PropertyAttribute를 상속받음 
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
