using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MonsterIndicator : MonoBehaviour
{
    [SerializeField] private Transform transform;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Transform playerTransform;

    public void Rotate(Vector3 targetPos)
    {
        targetPos.z = 0;
        Vector3 dir = targetPos - playerTransform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // 월드 회전을 직접 덮어씌움
        rectTransform.rotation = Quaternion.Euler(0f, 0f, angle+90f);
    }

    public void Rotate(Vector2 targetPos) => Rotate(new Vector3(targetPos.x, targetPos.y, 0));
    public void Rotate(Transform targetPos)
    {
        if (targetPos == null) 
            return;
        
        Rotate(targetPos.position);
    }


    public void TurnOn()
    {
        gameObject.SetActive(true);
    }

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }

    /*
    // 테스트용 마우스 홀딩
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f; // z축 고정
            Rotate(mouseWorldPos);
        }
    }
    */
}
