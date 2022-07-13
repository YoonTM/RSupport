using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Rock : Charactor
{
	public UnityEvent onRockCrashed { get; } = new UnityEvent();

	protected override void DiedCharactor()
	{
		base.DiedCharactor();
		onRockCrashed?.Invoke();
	}
}
