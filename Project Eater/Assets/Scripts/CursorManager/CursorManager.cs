using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// �� CursorType : ���콺 Ŀ���� Texture�� �ٲٴµ� ���
// ex) Skill�� �� ��, ���콺 Ŀ���� ������ ������� ���� 
public enum CursorType { Default, Select }

public class CursorManager : MonoBehaviour
{
    private static CursorManager instance;
    public static CursorManager Instance => instance;

    // Type�� ���� ������ Texture�� Cursor Texture�� ����
    [System.Serializable]
    private struct CursorData
    {
        public CursorType type;
        public Texture2D texture;
    }

    [SerializeField]
    private CursorData[] cursorDatas;

    private void Awake()
    {
        if (Instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            ChangeCursor(CursorType.Default);

            // �� ���� �� ChangeCursor(Default) �����ϵ��� �̺�Ʈ ���
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
            Destroy(gameObject); // �ߺ��� SaveSystem ����
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ChangeCursor(CursorType.Default);
    }

    public void ChangeCursor(CursorType newType)
    {
        if (newType == CursorType.Default)
        {
            // �� null : �⺻ Mouse Texture
            // �� Vector2.zero : Pivot(0, 0)
            // �� CursorMode.Auto : CursorMode�� Platform�� ���� �ڵ� ����
            var cursorTexture = cursorDatas.First(x => x.type == newType).texture;
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            // �� First : ������ ���տ��� ������ �����ϴ� ù ��° ��Ҹ� ��ȯ
            var cursorTexture = cursorDatas.First(x => x.type == newType).texture;
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }
    }
}
