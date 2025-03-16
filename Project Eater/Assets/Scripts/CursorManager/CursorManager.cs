using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// ※ CursorType : 마우스 커서의 Texture을 바꾸는데 사용
// ex) Skill을 쓸 때, 마우스 커서가 조준점 모양으로 변경 
public enum CursorType { Default, Select }

public class CursorManager : MonoBehaviour
{
    private static CursorManager instance;
    public static CursorManager Instance => instance;

    // Type에 따라서 정해진 Texture로 Cursor Texture을 변경
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

            // 씬 변경 시 ChangeCursor(Default) 실행하도록 이벤트 등록
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
            Destroy(gameObject); // 중복된 SaveSystem 제거
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ChangeCursor(CursorType.Default);
    }

    public void ChangeCursor(CursorType newType)
    {
        if (newType == CursorType.Default)
        {
            // ※ null : 기본 Mouse Texture
            // ※ Vector2.zero : Pivot(0, 0)
            // ※ CursorMode.Auto : CursorMode는 Platform에 따라 자동 선택
            var cursorTexture = cursorDatas.First(x => x.type == newType).texture;
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            // ※ First : 데이터 집합에서 조건을 만족하는 첫 번째 요소를 반환
            var cursorTexture = cursorDatas.First(x => x.type == newType).texture;
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }
    }
}
