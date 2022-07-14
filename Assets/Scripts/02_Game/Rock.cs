using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 화면의 깨지는 돌을 제어하는 스크립트. Charactor 스크립트를 상속받아 죽음 처리
/// </summary>
public class Rock : Charactor
{
	/// <summary>
	/// 돌의 죽음이 완료 됐을때 발생할 이벤트
	/// </summary>
	public UnityEvent onRockCrashed { get; } = new UnityEvent();

	/// <summary>
	/// 돌의 죽음 이벤트. 애니메이션 출력 후 이벤트 발생
	/// </summary>
	protected override void DiedCharactor()
	{
		base.DiedCharactor();
		onRockCrashed?.Invoke();
	}
}
