using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractionLatentSkill", menuName = "PlayerInteraction/LatentSkill")]
public class InteractionLatentSkill : InteractionPrefab
{
    public override void DoAction()
    {
        try
        {
            PlayerController.Instance.IsInterActive = true;
            PlayerController.Instance.enabled = false;
            GameManager.Instance.CinemachineTarget.enabled = false;

            var latentskillUI = GameObject.Find("UI").transform.Find("LatentSkill_Upgrade").GetComponent<LatentSkillUpgrade>();
            latentskillUI.SetUp(GameManager.Instance.player.OwnLatentSkills, GameManager.Instance.player.CurrentLatentSkill);
            latentskillUI.gameObject.SetActive(true);
        }
        catch
        {
            Debug.Log("Stat_Upgrade를 찾을 수 없습니다.");
        }
    }

    public override void ConditionCheck()
    {
        condition = true;
    }
}
