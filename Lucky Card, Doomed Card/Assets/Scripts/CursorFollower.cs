using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorFollower : MonoBehaviour
{
    [SerializeField] private Vector2 hotspotOffset = Vector2.zero; // 인스펙터에서 조정 가능
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Cursor.visible = false; // 기본 커서 숨김
    }


    void Update()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform,
            Input.mousePosition + (Vector3)hotspotOffset, // 오프셋 적용
            null,
            out pos);
        rectTransform.anchoredPosition = pos;
    }

} 
