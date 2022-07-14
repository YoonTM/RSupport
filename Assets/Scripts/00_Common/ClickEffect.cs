using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 버튼 클릭 이펙트 컴포넌트
/// </summary>
public class ClickEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform rectTransform { get { return (RectTransform)transform; } }

    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        rectTransform.localScale = Vector3.one;
    }
}