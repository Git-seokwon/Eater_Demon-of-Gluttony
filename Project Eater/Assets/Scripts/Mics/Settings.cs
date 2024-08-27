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
	// → 60 프레임에 맞추도록 고정(목표 프레임 속도 : 60)
	public const int targetFrameRateForPathFind = 60;
	// targetPosition(playerPosition)과 playerDistanceToRebuildPath만큼 차이가 난다면 경로를 갱신 
	public const float playerDistanceToRebuildPath = 3f;
	public const float enemyPathRebuildCooldown = 2f;
    #endregion
}
