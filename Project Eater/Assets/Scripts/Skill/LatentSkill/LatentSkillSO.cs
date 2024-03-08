using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LatentSkillSO_", menuName = "Scriptable Object/Skill/LatentSkill")]
public class LatentSkillSO : ScriptableObject
{
	#region HEADER BASE INFO
	[Space(10)]
	[Header("Latent Skill Base Info")]
	#endregion
	#region ToolTip
	[Tooltip("Latent Skill Name")]
	#endregion
	public string latentSkillName;

	#region HEADER STARTING SKILL
	[Space(10)]
	[Header("Player Starting Skill(Basic Skill)")]
	#endregion
	#region ToolTip
	[Tooltip("Projectile Prefab")]
	#endregion
	public GameObject projectilePrefab;
	#region ToolTip
	[Tooltip("Projectile Speed")]
	#endregion
	public float projectileSpeed;
	#region ToolTip
	[Tooltip("Projectile Range")]
	#endregion
	public float projectileRange;
	#region ToolTip
	[Tooltip("Projectile Fire Rate")]
	#endregion
	public float projectileFireRate;
	#region ToolTip
	[Tooltip("Projectile Charging Time, if No charging time, startingSkillChargingTime is 0")]
	#endregion
	public float startingSkillChargingTime;
	#region ToolTip
	[Tooltip("Starting Skill Sound Effect")]
	#endregion
	public SoundEffectSO startingSkillSoundEffect;

    // Latent Skill Trait�� �� Latent Skill ��ũ��Ʈ�� ������ �ۼ�!
	// Ultimate Skill�� �� Latent Skill ��ũ��Ʈ�� ������ �ۼ�!
}
