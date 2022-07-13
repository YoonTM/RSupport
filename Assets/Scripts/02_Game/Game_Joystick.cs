using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Game_Joystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IEndDragHandler
{
	[SerializeField] RectTransform joyStickBackground = null;
	[SerializeField] RectTransform joyStickBar = null;

	public UnityAction<Vector2> moveVecUpdate { get; set; }
	private Vector2 moveValue;

	private float fRadius;
	private bool isControlling;
	private void Start()
	{
		Input.multiTouchEnabled = true;
	}

	private void OnDestroy()
	{
		Input.multiTouchEnabled = false;
	}

	void OnTouch(Vector2 vecTouch)
	{
		Vector2 vec = new Vector2(vecTouch.x - joyStickBackground.position.x, vecTouch.y - joyStickBackground.position.y);
		vec = Vector2.ClampMagnitude(vec, fRadius);
		joyStickBar.localPosition = vec;
		moveValue = vec / fRadius;
		moveVecUpdate?.Invoke(moveValue);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		joyStickBar.localPosition = Vector2.zero;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		fRadius = joyStickBackground.rect.width * 0.5f;
		OnTouch(eventData.position);
	}

	public void OnDrag(PointerEventData eventData)
	{
		OnTouch(eventData.position);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		moveVecUpdate?.Invoke(Vector2.zero);
	}
}