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
        // �̸� �κ��丮 â���� ����� ���� �� 
        // �� ���� UpdateInventoryUI���� null�� �� Slot�� ��Ȱ��ȭ ���� ����
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
        // ���� ���������� ������ ���������� ������ ��ų�� �ε���
        int endIndex = Mathf.Min(slotsPerPage, ownSkills.Count);

        for (int i = 0; i < slots.Count; i++)
        {
            // �ش� Index�� endIndex(InventorySkill�� ������ Index)���� ������ ��ų ���� ����
            if (i < endIndex)
                slots[i].Setup(ownSkills[i]);
            // endIndex ���� ũ�� �� ĭ�̱� ������ null�� �����Ѵ�. 
            else
                slots[i].Setup(null);
        }
    }
}
