using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(UnderlineTitleAttribute))] // DecoratorDrawer나 PropertyDrawer를 만들 때, 해당 Attribute에 Draw할 Class를 넣어줘야 한다. 
public class UnderlineTitleDrawer : DecoratorDrawer // Attribute를 그려주기 위해 상속
{
    // position : GUI가 그려질 위치 
    public override void OnGUI(Rect position)
    {
        // ★ 새로 그려주기 때문에 base로 안 받아온다. ★

        // ※ attribute : DecoratorDrawer에 있는 변수로, 우리가 IdentifiedObject를 작성할 때 
        //              : 실제 IdentifiedObject가 들어있던 target 변수처럼 실제 Attribute 객체가 들어있다. 
        // → attribute를 UnderlineTitleAttribute로 캐스팅
        var attributeAsUnderlineTitle = attribute as UnderlineTitleAttribute;

        // 들여쓰기가 적용된 Position 
        // → 유니티의 GUI에도 들여쓰기 개념이 존재하고, 기본적으로 Position에 들여쓰기 수치가 적용이 안되어 있기 때문에 
        //    우리가 직접 들여쓰기 수치를 적용시켜서 GUI가 올바른 위치에 그려지게 해준다. 
        position = EditorGUI.IndentedRect(position);

        position.height = EditorGUIUtility.singleLineHeight;

        // Y축을 attribute에 설정된 Space 값만큼 내려준다. 
        // → Rect X Y 관련 사이트 :  https://ansohxxn.github.io/unitydocs/rect/
        position.y += attributeAsUnderlineTitle.Space;

        // position에 title을 Bold Style로 그림
        GUI.Label(position, attributeAsUnderlineTitle.Title, EditorStyles.boldLabel);

        // 한 줄 이동
        position.y += EditorGUIUtility.singleLineHeight;
        // 높이(두께)는 1
        position.height = 1f;
        // 회색 Box를 그림
        // → 높이가 1이라서 Box가 아닌 회색 선이 그려짐
        EditorGUI.DrawRect(position, Color.gray);
    }

    // 우리가 위에서 그린 GUI의 총 높이를 반환하는 함수
    // → 이 함수가 반환하는 값을 기준으로 지금 GUI가 다 그려지고 난 뒤, 
    //    다음 GUI가 어디서부터 그려질지 결정 된다. 
    // → 잘못된 값을 반환하면 지금 그린 GUI와 다음 GUI가 겹쳐서 그려지거나 
    //    반대로 아예 확 떨어져서 그려지는 문제가 생김
    // → 그렇기에 수치 계산을 잘해야 한다. (이거는 계속 값 바꾸면서 확인해야 함)
    public override float GetHeight()
    {
        var attributeAsUnderlineTitle = attribute as UnderlineTitleAttribute;

        // ※ standardVerticalSpacing : GUI들 사이의 여유 공간의 높이 
        //                            : 1로 하면 다음 GUI랑 딱 붙어서 그려지기 때문에 * 2를 해준 것이다. 
        // 설정한 Attribute Space + 기본 GUI 높이 + (기본 GUI 간격 * 2)
        return attributeAsUnderlineTitle.Space + EditorGUIUtility.singleLineHeight + (EditorGUIUtility.standardVerticalSpacing * 2);
    }
}
