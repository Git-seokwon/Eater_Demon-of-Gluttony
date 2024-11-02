using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillChoicesTest : MonoBehaviour
{
    [SerializeField]
    private int skillCombinationCount;
    [SerializeField]
    private int skillUpgradeCount;
    [SerializeField]
    private int skillAcquisitionCount;

    private int skillChoices = 4;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SetSkillChoices();
        }
    }

    private void SetSkillChoices()
    {
        int remainChoices = skillChoices;
        int goodsChoices = 0;
        List<SkillCombinationSlotNode> skills = new();

        int skillCombinationChoices = 0;
        int skillUpgradeChoices = 0;
        int skillAcquisitionChoices = 0;

        // ��ų ������ ��ü�� ������ ���, Ex) ȹ�� 1, ��ȭ 1, ���� 1�� ������ ��, 
        // ������ 1���� �������� ��ȭ(goods)�� �ִ� �������� ó���Ѵ�. 
        // �� �ش� ��쿡�� while ���� �� �ʿ䰡 ���� ������ ������ �������� �� ������ List�� Count�� �ʱ�ȭ �����ش�. 
        if (skillCombinationCount + skillUpgradeCount + skillAcquisitionCount <= skillChoices)
        {
            goodsChoices = skillChoices - (skillCombinationCount + skillUpgradeCount + skillAcquisitionCount);

            skillCombinationChoices = skillCombinationCount;
            skillUpgradeChoices = skillUpgradeCount;
            skillAcquisitionChoices = skillAcquisitionCount;
        }
        else
        {
            while (remainChoices > 0)
            {
                // ������ �迭 �� �ϳ��� �����ϰ� ����
                int randomSelection;
                int weightedRandom = Random.Range(0, 7);

                // 4/7�� Ȯ��
                if (weightedRandom < 4)
                    randomSelection = 0;
                // 2/7�� Ȯ��
                else if (weightedRandom < 6)
                    randomSelection = 1;
                // 1/7�� Ȯ��
                else
                    randomSelection = 2;

                int skillCount = 0;

                switch (randomSelection)
                {
                    case 0:
                        skillCount = skillCombinationCount - skillCombinationChoices;
                        break;
                    case 1:
                        skillCount = skillUpgradeCount - skillUpgradeChoices;
                        break;
                    case 2:
                        skillCount = skillAcquisitionCount - skillAcquisitionChoices;
                        break;
                }

                if (skillCount > 0)
                {
                    int choices;
                    remainChoices = CalculateChoices(remainChoices, skillCount, out choices);

                    // ���� �������� �ش� �������� �߰�
                    switch (randomSelection)
                    {
                        case 0:
                            skillCombinationChoices += choices;
                            break;
                        case 1:
                            skillUpgradeChoices += choices;
                            break;
                        case 2:
                            skillAcquisitionChoices += choices;
                            break;
                    }
                }

                // ��� �������� �����ϸ� �ݺ��� ����
                if (remainChoices <= 0)
                    break;
            }
        }

        Debug.Log("�÷��̾� ������");
        Debug.Log("���� : " + skillCombinationChoices);
        Debug.Log("��ȭ : " + skillUpgradeChoices);
        Debug.Log("ȹ�� : " + skillAcquisitionChoices);
        Debug.Log("��ȭ : " + goodsChoices);
    }

    private int CalculateChoices(int remainChoices, int skillCount, out int choices)
    {
        choices = Random.Range(1, Mathf.Min(remainChoices, skillCount) + 1);
        remainChoices -= choices;
        return remainChoices;
    }
}
