using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    private SkillSystem skillSystem;

    void Start()
    {
        PlayerController.Instance.SetPlayerMode(PlayerMode.Devil);
        var player = GetComponent<PlayerEntity>();
        skillSystem = GetComponent<SkillSystem>();

        if (player.SkillSystem.defaultSkills.Length > 0)
        {
            player.SkillSystem.InitSkillSlots();

            for (int i = 0; i < player.SkillSystem.defaultSkills.Length; i++)
            {
                var clone = player.SkillSystem.Register(player.SkillSystem.defaultSkills[i]);
                player.SkillSystem.Equip(clone, i + 1);
            }

            player.SetUpLatentSkill();
            player.AcquireLatentSkill(1);
            player.ChangeLatentSkill(0);

            player.SkillSystem.SetupLatentSkills(player.CurrentLatentSkill.Level);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            skillSystem.ReSetPlayerSkills();

            /*var stats = GetComponent<Stats>();

            stats.IncreaseDefaultValue(stats.FullnessStat, 100f);*/
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            foreach (var skill in skillSystem.defaultSkills)
            {
                skillSystem.SkillLevelUp(skill);
            }
        }
    }
}
