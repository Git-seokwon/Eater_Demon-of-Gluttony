using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Test
{
    // ※ AddComponentMenu : 사용자 스크립트의 컴포넌트 메뉴 구성
    // Add Component -> Test -> StatTest
    [AddComponentMenu("Test/StatTest")]
    public class StatTest : MonoBehaviour
    {
        // ※ 컨텍스트 메뉴 : 인스펙터창의 속성 버튼
        // → 인스펙터창 속성 버튼 누르면 Test가 뜨고, Test 누르면 아래 Test 함수 실행 
        [ContextMenu("Test")]
        private void Test()
        {
            Debug.Log("<color=yellow>[StatTest] Start</color>");

            // ※ ScriptableObject.CreateInstance : ScriptableObject 생성
            //                                    : new로 생성하지 않은 이유는 MonoBehaviour와 마찬가지로,
            //                                      유니티의 Serialize 구조를 경유해서 오브젝트를 만들기 때문이다. 
            var stat = ScriptableObject.CreateInstance<Stat>();
            stat.MaxValue = float.MaxValue;

            // Stat에 Test라는 이름으로 10의 Bonus Value를 추가해 준다. 
            stat.SetBonusValue("Test", 10f);
            // ※ Assert 
            // → Assert Class는 Dubugging을 위한 Error 검출 함수들의 모음
            // → 기본적으로 Editor에서만 작동하는 함수이다. 즉, Game을 Build하게 되면 이 구문 자체가 무시된다. 
            // → Dubug 클래스는 Runtime에도 실행되기 때문에 Performance 비용 계속 소모된다. 하지만, Assert는 그런 비용 문제가 발생하지 않다. 
            //    그래서 실 개발에서 굉장히 많이 쓰인다.
            // → Assert문을 작성해서 빠르게 Error를 검출할 수 있게 코딩하는 걸 방어적 코딩이라 한다. 
            // → Assert라는 건 Unity나 C#만의 개념이 아니라 유명한 언어들에는 다 있다. 
            // ※ Assert.IsTrue : 왼쪽 Condition이 true이면 함수를 그냥 빠져나오고 false면 Test가 중단되면서, 오른쪽의 Text를 Console창에 Error로 띄움
            Assert.IsTrue(stat.ContainBonusValue("Test"), "Test Bonus Value가 없습니다.");
            Assert.IsTrue(Mathf.Approximately(stat.GetBonusValue("Test"), 10f), "Stat의 Test Bonus Value가 10이 아닙니다.");
            Debug.Log($"Test Bonus Value: {stat.GetBonusValue("Test")}");

            Assert.IsTrue(stat.RemoveBonusValue("Test"), "Test Bonus Value의 삭제 실패");
            // Assert.IsTrue 반대 
            Assert.IsFalse(stat.ContainBonusValue("Test"), "Test Bonus Value를 삭제하였으나 아직 남아있습니다.");
            Debug.Log("Remove Test Bonus Value");

            // Sub Key에 관한 Test
            stat.SetBonusValue("Test", "Test2", 10f);
            Assert.IsTrue(stat.ContainBonusValue("Test", "Test2"), "Test-Test2 Bonus Value가 없습니다.");
            Assert.IsTrue(Mathf.Approximately(stat.GetBonusValue("Test", "Test2"), 10f), "Test-Test2 Bonus Value가 10이 아닙니다.");
            Debug.Log($"Test-Test2 Bonus Value: {stat.GetBonusValue("Test", "Test2")}");

            Assert.IsTrue(stat.RemoveBonusValue("Test", "Test2"), "Test-Test2 Bonus Value의 삭제 실패");
            Assert.IsFalse(stat.ContainBonusValue("Test", "Test2"), "Test-Test2 Bonus Value를 삭제하였으나 아직 남아있습니다.");
            Debug.Log("Remove Test-Test2 Bonus Value");

            stat.RemoveBonusValue("Test");
            Debug.Log("Remove Test Bonus Value");

            // Bonus Value 합계 검사 
            stat.SetBonusValue("Test", 100f);
            Debug.Log("Set Test Bonus: " + stat.GetBonusValue("Test"));
            stat.SetBonusValue("Test2", 100f);
            Debug.Log("Set Test2 Bonus: " + stat.GetBonusValue("Test2"));
            Assert.IsTrue(Mathf.Approximately(stat.BonusValue, 200f), "Bonus Value의 합계가 200이 아닙니다.");
            Debug.Log("Total Bonus Value: 200");

            // 토탈 Value = DefaultValue + BonusValue 검사 
            stat.DefaultValue = 100f;
            Debug.Log("Set Default Value: " + stat.DefaultValue);
            Assert.IsTrue(Mathf.Approximately(stat.Value, 300f), "Total Value가 300이 아닙니다.");
            Debug.Log("Value: 300");

            if (Application.isPlaying)
                Destroy(stat);
            else
                DestroyImmediate(stat);

            Debug.Log("<color=green>[StatTest] Success</color>");
        }
    }
}
