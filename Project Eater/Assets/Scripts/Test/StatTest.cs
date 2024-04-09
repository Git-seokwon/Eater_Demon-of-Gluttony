using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Test
{
    // �� AddComponentMenu : ����� ��ũ��Ʈ�� ������Ʈ �޴� ����
    // Add Component -> Test -> StatTest
    [AddComponentMenu("Test/StatTest")]
    public class StatTest : MonoBehaviour
    {
        // �� ���ؽ�Ʈ �޴� : �ν�����â�� �Ӽ� ��ư
        // �� �ν�����â �Ӽ� ��ư ������ Test�� �߰�, Test ������ �Ʒ� Test �Լ� ���� 
        [ContextMenu("Test")]
        private void Test()
        {
            Debug.Log("<color=yellow>[StatTest] Start</color>");

            // �� ScriptableObject.CreateInstance : ScriptableObject ����
            //                                    : new�� �������� ���� ������ MonoBehaviour�� ����������,
            //                                      ����Ƽ�� Serialize ������ �����ؼ� ������Ʈ�� ����� �����̴�. 
            var stat = ScriptableObject.CreateInstance<Stat>();
            stat.MaxValue = float.MaxValue;

            // Stat�� Test��� �̸����� 10�� Bonus Value�� �߰��� �ش�. 
            stat.SetBonusValue("Test", 10f);
            // �� Assert 
            // �� Assert Class�� Dubugging�� ���� Error ���� �Լ����� ����
            // �� �⺻������ Editor������ �۵��ϴ� �Լ��̴�. ��, Game�� Build�ϰ� �Ǹ� �� ���� ��ü�� ���õȴ�. 
            // �� Dubug Ŭ������ Runtime���� ����Ǳ� ������ Performance ��� ��� �Ҹ�ȴ�. ������, Assert�� �׷� ��� ������ �߻����� �ʴ�. 
            //    �׷��� �� ���߿��� ������ ���� ���δ�.
            // �� Assert���� �ۼ��ؼ� ������ Error�� ������ �� �ְ� �ڵ��ϴ� �� ����� �ڵ��̶� �Ѵ�. 
            // �� Assert��� �� Unity�� C#���� ������ �ƴ϶� ������ ���鿡�� �� �ִ�. 
            // �� Assert.IsTrue : ���� Condition�� true�̸� �Լ��� �׳� ���������� false�� Test�� �ߴܵǸ鼭, �������� Text�� Consoleâ�� Error�� ���
            Assert.IsTrue(stat.ContainBonusValue("Test"), "Test Bonus Value�� �����ϴ�.");
            Assert.IsTrue(Mathf.Approximately(stat.GetBonusValue("Test"), 10f), "Stat�� Test Bonus Value�� 10�� �ƴմϴ�.");
            Debug.Log($"Test Bonus Value: {stat.GetBonusValue("Test")}");

            Assert.IsTrue(stat.RemoveBonusValue("Test"), "Test Bonus Value�� ���� ����");
            // Assert.IsTrue �ݴ� 
            Assert.IsFalse(stat.ContainBonusValue("Test"), "Test Bonus Value�� �����Ͽ����� ���� �����ֽ��ϴ�.");
            Debug.Log("Remove Test Bonus Value");

            // Sub Key�� ���� Test
            stat.SetBonusValue("Test", "Test2", 10f);
            Assert.IsTrue(stat.ContainBonusValue("Test", "Test2"), "Test-Test2 Bonus Value�� �����ϴ�.");
            Assert.IsTrue(Mathf.Approximately(stat.GetBonusValue("Test", "Test2"), 10f), "Test-Test2 Bonus Value�� 10�� �ƴմϴ�.");
            Debug.Log($"Test-Test2 Bonus Value: {stat.GetBonusValue("Test", "Test2")}");

            Assert.IsTrue(stat.RemoveBonusValue("Test", "Test2"), "Test-Test2 Bonus Value�� ���� ����");
            Assert.IsFalse(stat.ContainBonusValue("Test", "Test2"), "Test-Test2 Bonus Value�� �����Ͽ����� ���� �����ֽ��ϴ�.");
            Debug.Log("Remove Test-Test2 Bonus Value");

            stat.RemoveBonusValue("Test");
            Debug.Log("Remove Test Bonus Value");

            // Bonus Value �հ� �˻� 
            stat.SetBonusValue("Test", 100f);
            Debug.Log("Set Test Bonus: " + stat.GetBonusValue("Test"));
            stat.SetBonusValue("Test2", 100f);
            Debug.Log("Set Test2 Bonus: " + stat.GetBonusValue("Test2"));
            Assert.IsTrue(Mathf.Approximately(stat.BonusValue, 200f), "Bonus Value�� �հ谡 200�� �ƴմϴ�.");
            Debug.Log("Total Bonus Value: 200");

            // ��Ż Value = DefaultValue + BonusValue �˻� 
            stat.DefaultValue = 100f;
            Debug.Log("Set Default Value: " + stat.DefaultValue);
            Assert.IsTrue(Mathf.Approximately(stat.Value, 300f), "Total Value�� 300�� �ƴմϴ�.");
            Debug.Log("Value: 300");

            if (Application.isPlaying)
                Destroy(stat);
            else
                DestroyImmediate(stat);

            Debug.Log("<color=green>[StatTest] Success</color>");
        }
    }
}
