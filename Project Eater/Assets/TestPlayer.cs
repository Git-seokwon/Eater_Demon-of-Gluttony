using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    void Start()
    {
        PlayerController.Instance.SetPlayerMode(PlayerMode.Devil);
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
