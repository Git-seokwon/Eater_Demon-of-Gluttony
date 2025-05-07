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
    // 한 페이지에 보일 슬롯 최대 갯수
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
        // 미리 인벤토리 창들을 만들어 놓는 것 
        // → 이후 UpdateInventoryUI에서 null이 들어간 Slot은 비활성화 해줄 것임 
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
        // 현재 페이지에서 실제로 마지막으로 보여줄 스킬의 인덱스
        int endIndex = Mathf.Min(startIndex + slotsPerPage, ownSkills.Count);

        for (int i = 0; i < slots.Count; i++)
        {
            // 해당 Index가 endIndex(InventorySkill의 마지막 Index)보다 작으면 스킬 정보 대입
            if (i + startIndex < endIndex)
                slots[i].Setup(ownSkills[i + startIndex], this);
            // endIndex 보다 크면 빈 칸이기 때문에 null을 대입한다. 
            else
                slots[i].Setup(null, this);
        }

        // Ex) ownSkills.Count가 40이면, Mathf.CeilToInt(40.000/30)이 되므로
        //     2라는 값이 나오게 되어 최대 Page 수를 보여줄 수 있다.  
        pageNumber.text = $"{currentPage + 1} / {Mathf.CeilToInt((float)ownSkills.Count / slotsPerPage)}";
        // currentPage가 0이면 이전 페이지는 없기 때문에 상호작용 하지 못하게 한다. 
        prevButton.interactable = currentPage > 0;
        // 현재 페이지에 아직 표시되지 않은 스킬이 남아있으면(endIndex < ownSkills.Count) nextButton이 활성화
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
        // 플레이어가 가지고 있는 스킬 수가 현재까지 보여준 페이지에 있는 스킬 수보다 많을 경우
        if ((currentPage + 1) * slotsPerPage < ownSkills.Count)
        {
            currentPage++;
            UpdateInventoryUI();
        }
    }

    private void Close()
    {
        // 플레이어 조작 가능 & 게임 시간 진행
        GameManager.Instance.CinemachineTarget.enabled = true;
        PlayerController.Instance.IsInterActive = false;
        PlayerController.Instance.enabled = true;
        Time.timeScale = 1f;

        GameManager.Instance.player.isLevelUp = false;
        GameManager.Instance.player.Animator.speed = 1f;

        gameObject.SetActive(false);
    }
}
