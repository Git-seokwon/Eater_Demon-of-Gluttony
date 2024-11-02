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

        // 스킬 선택지 자체가 부족한 경우, Ex) 획득 1, 강화 1, 조합 1만 가능할 때, 
        // 나머지 1개의 선택지를 재화(goods)를 주는 선택지로 처리한다. 
        // → 해당 경우에는 while 문을 돌 필요가 없기 때문에 선택지 종류들을 각 선택지 List의 Count로 초기화 시켜준다. 
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
                // 선택지 배열 중 하나를 랜덤하게 선택
                int randomSelection;
                int weightedRandom = Random.Range(0, 7);

                // 4/7의 확률
                if (weightedRandom < 4)
                    randomSelection = 0;
                // 2/7의 확률
                else if (weightedRandom < 6)
                    randomSelection = 1;
                // 1/7의 확률
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

                    // 남은 선택지를 해당 선택지에 추가
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

                // 모든 선택지를 소진하면 반복문 종료
                if (remainChoices <= 0)
                    break;
            }
        }

        Debug.Log("플레이어 선택지");
        Debug.Log("조합 : " + skillCombinationChoices);
        Debug.Log("강화 : " + skillUpgradeChoices);
        Debug.Log("획득 : " + skillAcquisitionChoices);
        Debug.Log("재화 : " + goodsChoices);
    }

    private int CalculateChoices(int remainChoices, int skillCount, out int choices)
    {
        choices = Random.Range(1, Mathf.Min(remainChoices, skillCount) + 1);
        remainChoices -= choices;
        return remainChoices;
    }
}
