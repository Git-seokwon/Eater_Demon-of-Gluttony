using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillInventory : MonoBehaviour
{
    [SerializeField]
    private GameObject slotPrefab;
    [SerializeField]
    private Transform slotsParent;

    [Space(10)]
    [SerializeField]
    private Button prevButton;
    [SerializeField]
    private Button nextButton;
    [SerializeField]
    private TextMeshProUGUI pageNumber;

    [Space(10)]
    [SerializeField]
    private Button closeButton;

    [field : SerializeField]
    public CanvasGroup activeSkills {  get; private set; }
    [field: SerializeField]
    public CanvasGroup passiveSkills { get; private set; }

    private List<Skill> ownSkills = new List<Skill>();
    private List<InventorySlot> slots = new List<InventorySlot>();

    private int currentPage = 0;
    // �� �������� ���� ���� �ִ� ����
    private int slotsPerPage = 30;

    private void OnEnable()
    {
        GameManager.Instance.player.PlayerMovement.Stop();
        GameManager.Instance.player.isLevelUp = true;
        GameManager.Instance.player.Animator.speed = 0f;
        GameManager.Instance.CinemachineTarget.enabled = false;
        PlayerController.Instance.IsInterActive = true;
        PlayerController.Instance.enabled = false;
        Time.timeScale = 0f;

        prevButton.onClick.AddListener(ShowPrevPage);
        nextButton.onClick.AddListener(ShowNextPage);
        closeButton.onClick.AddListener(Close);

        ownSkills = GameManager.Instance.player.SkillSystem.OwnSkills.
                    Where(skill => skill.Grade != SkillGrade.Latent).ToList();
        
        PopulateSlots();
        UpdateInventoryUI();
    }

    private void OnDisable()
    {
        prevButton.onClick.RemoveAllListeners();
        nextButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();

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

            var slot = slotGO.GetComponent<InventorySlot>();
            slots.Add(slot);
        }
    }

    private void UpdateInventoryUI()
    {
        int startIndex = currentPage * slotsPerPage;
        // ���� ���������� ������ ���������� ������ ��ų�� �ε���
        int endIndex = Mathf.Min(startIndex + slotsPerPage, ownSkills.Count);

        for (int i = 0; i < slots.Count; i++)
        {
            // �ش� Index�� endIndex(InventorySkill�� ������ Index)���� ������ ��ų ���� ����
            if (i + startIndex < endIndex)
                slots[i].Setup(ownSkills[i + startIndex], this);
            // endIndex ���� ũ�� �� ĭ�̱� ������ null�� �����Ѵ�. 
            else
                slots[i].Setup(null, this);
        }

        // Ex) ownSkills.Count�� 40�̸�, Mathf.CeilToInt(40.000/30)�� �ǹǷ�
        //     2��� ���� ������ �Ǿ� �ִ� Page ���� ������ �� �ִ�.  
        pageNumber.text = $"{currentPage + 1} / {Mathf.CeilToInt((float)ownSkills.Count / slotsPerPage)}";
        // currentPage�� 0�̸� ���� �������� ���� ������ ��ȣ�ۿ� ���� ���ϰ� �Ѵ�. 
        prevButton.interactable = currentPage > 0;
        // ���� �������� ���� ǥ�õ��� ���� ��ų�� ����������(endIndex < ownSkills.Count) nextButton�� Ȱ��ȭ
        nextButton.interactable = endIndex < ownSkills.Count;
    }

    private void ShowPrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateInventoryUI();
        }
    }

    private void ShowNextPage()
    {
        // �÷��̾ ������ �ִ� ��ų ���� ������� ������ �������� �ִ� ��ų ������ ���� ���
        if ((currentPage + 1) * slotsPerPage < ownSkills.Count)
        {
            currentPage++;
            UpdateInventoryUI();
        }
    }

    private void Close()
    {
        // �÷��̾� ���� ���� & ���� �ð� ����
        GameManager.Instance.CinemachineTarget.enabled = true;
        PlayerController.Instance.IsInterActive = false;
        PlayerController.Instance.enabled = true;
        Time.timeScale = 1f;

        GameManager.Instance.player.isLevelUp = false;
        GameManager.Instance.player.Animator.speed = 1f;

        gameObject.SetActive(false);
    }
}
