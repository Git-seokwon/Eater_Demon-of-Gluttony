using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageEndSkills : MonoBehaviour
{
    [SerializeField]
    private GameObject slotPrefab;
    [SerializeField]
    private Transform slotsParent;

    private List<Skill> ownSkills = new List<Skill>();
    private List<DisplaySkillSlot> slots = new List<DisplaySkillSlot>();

    private int slotsPerPage = 33;

    private void OnEnable()
    {
        ownSkills = GameManager.Instance.player.SkillSystem.OwnSkills.
                    Where(skill => skill.Grade != SkillGrade.Latent).ToList();

        PopulateSlots();
        UpdateInventoryUI();
    }

    private void OnDisable()
    {
        slots.Clear();
        ownSkills.Clear();
    }

    private void PopulateSlots()
    {
        // 미리 인벤토리 창들을 만들어 놓는 것 
        // → 이후 UpdateInventoryUI에서 null이 들어간 Slot은 비활성화 해줄 것임
        for (int i = 0; i < slotsPerPage; i++)
        {
            var slotGO = PoolManager.Instance.ReuseGameObject(slotPrefab, Vector3.zero, Quaternion.identity);
            slotGO.transform.SetParent(slotsParent, false);

            var slot = slotGO.GetComponent<DisplaySkillSlot>();
            slots.Add(slot);
        }
    }

    private void UpdateInventoryUI()
    {
        // 현재 페이지에서 실제로 마지막으로 보여줄 스킬의 인덱스
        int endIndex = Mathf.Min(slotsPerPage, ownSkills.Count);

        for (int i = 0; i < slots.Count; i++)
        {
            // 해당 Index가 endIndex(InventorySkill의 마지막 Index)보다 작으면 스킬 정보 대입
            if (i < endIndex)
                slots[i].Setup(ownSkills[i]);
            // endIndex 보다 크면 빈 칸이기 때문에 null을 대입한다. 
            else
                slots[i].Setup(null);
        }
    }
}
