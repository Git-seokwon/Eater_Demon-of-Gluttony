using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    void Start()
    {
        PlayerController.Instance.SetPlayerMode(PlayerMode.Devil);
        var player = GetComponent<PlayerEntity>();

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
            var stats = GetComponent<Stats>();

            stats.IncreaseDefaultValue(stats.FullnessStat, 100f);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            var skillsystem = GetComponent<SkillSystem>();

            foreach (var skill in skillsystem.defaultSkills)
            {
                skillsystem.SkillLevelUp(skill);
            }
        }
    }
}
