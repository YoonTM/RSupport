using UnityEngine;
using UnityEngine.EventSystems;


//  Button 컴포넌트에 추가하여 클릭 이펙트를 나타내는 스크립트.
//  이벤트에따라 스케일 변화.
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