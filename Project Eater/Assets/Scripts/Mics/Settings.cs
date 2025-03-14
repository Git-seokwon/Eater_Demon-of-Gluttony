using UnityEngine;

public static class Settings 
{
	#region GAMEOBJECT TAG
	public const string playerTag = "Player";
	#endregion

	#region ROOM SETTINGS
	public const float fadeInTime = 0.5f; // time to fade in the room
	public const float fadeOutTime = 0.5f; // time to fade out the room
	#endregion

	#region ASTAR PATHFINDING PARAMETERS
	// Astar Path 최적화 변수 
	// → 60 프레임에 맞추도록 고정(목표 프레임 속도 : 120)
	public const float neighborUpdateInterval = 2f;
	public const float enemyPathRebuildCooldown = 2f;
	#endregion

	#region EXP
	public const int eliteEXP = 20;
	#endregion

	#region MEET
	public const float itemMoveSpeed = 5f;
	public const float bounceDistance = 0.5f;
	public const float bounceDuration = 0.2f;
	public const float itemCloseEnoughDistance = 0.3f;
	#endregion

	#region FLOWFIELD
	public const int obstacle = 255;
    #endregion

    #region AUDIO
    public const float musicFadeOutTime = 0.5f; // Default Music Fade Out Transition
    public const float musicFadeInTime = 0.5f; // Default Music Fade In Transition
	#endregion

	#region SuperArmor
	public const float superArmorDuration = 2f;
	#endregion
}
