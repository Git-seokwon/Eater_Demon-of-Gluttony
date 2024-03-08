using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatSO", menuName = "Scriptable Object/Player/Stat")]
public class PlayerStatSO : ScriptableObject
{
	#region HEADER PLAYER BASIC INFO
	[Space(10)]
	[Header("PLAYER BASIC INFO")]
	#endregion
	#region ToolTip
	[Tooltip("Prefab gameObject for the Player")]
	#endregion
	public GameObject playerPrefab;
	#region ToolTip
	[Tooltip("Player Runtime Animator Controller")]
	#endregion
	public RuntimeAnimatorController animatorController;

	#region HEADER PLAYER STAT
	[Space(10)]
	[Header("PLAYER STATS")]
	#endregion
	#region ToolTip
	[Tooltip("Player Starting Hunger Amount")]
	#endregion
	public int hunger;
	#region ToolTip
	[Tooltip("Player Attack Power")]
	#endregion
	public float attackPower;
	#region ToolTip
	[Tooltip("Player Attack Speed")]
	#endregion
	public float attackSpeed;
	#region ToolTip
	[Tooltip("Player Armor : decrease monster damage by int")]
	#endregion
	public int armor;
	#region Tooltip
	[Tooltip("Player Crit Rate")]
	#endregion
	public float critRate;
	#region Tooltip
	[Tooltip("Player Crit Multiple")]
	#endregion
	public float critMultiple;
	#region ToolTip
	[Tooltip("Player Move Speed")]
	#endregion
	public float moveSpeed;
	#region Tooltip
	[Tooltip("Player Basic Attack Range")]
	#endregion
	public float basicAttackRange;
	#region ToolTip
	[Tooltip("Player Hunger Ascent Speed")]
	#endregion
	public int hungerAscentSpeed;
	#region ToolTip
	[Tooltip("Player Select Refresh")]
	#endregion
	public int refresh;
	#region TooTip
	[Tooltip("Player Skill Haste")]
	#endregion
	public float skillHaste;
	#region ToolTip
	[Tooltip("Player Status Effect Step : in Status Plan, 'Obstruct'")]
	#endregion
	public int statusEffectAbility;
	#region ToolTip
	[Tooltip("Player Fullness : EXP")]
	#endregion
	public int fullness;
	#region Tooltip
	[Tooltip("Player Summon Damage")]
	#endregion
	public float summonDamage;
	#region ToolTip
	[Tooltip("Player Luck : in Status Plan, 'Luck'")]
	#endregion
	public int luck;
	#region Tooltip
	[Tooltip("Player Magnet : Meet Range Up")]
	#endregion
	public float magnetRange;
}
