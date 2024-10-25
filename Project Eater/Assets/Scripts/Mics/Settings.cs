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
	// Astar Path ����ȭ ���� 
	// �� 60 �����ӿ� ���ߵ��� ����(��ǥ ������ �ӵ� : 60)
	public const int targetFrameRateForPathFind = 60;
	public const float enemyPathRebuildCooldown = 2f;
    #endregion
}
