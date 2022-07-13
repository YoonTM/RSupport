using UnityEngine;
using UnityEngine.EventSystems;


//  Button ������Ʈ�� �߰��Ͽ� Ŭ�� ����Ʈ�� ��Ÿ���� ��ũ��Ʈ.
//  �̺�Ʈ������ ������ ��ȭ.
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