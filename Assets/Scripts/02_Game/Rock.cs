using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ȭ���� ������ ���� �����ϴ� ��ũ��Ʈ. Charactor ��ũ��Ʈ�� ��ӹ޾� ���� ó��
/// </summary>
public class Rock : Charactor
{
	/// <summary>
	/// ���� ������ �Ϸ� ������ �߻��� �̺�Ʈ
	/// </summary>
	public UnityEvent onRockCrashed { get; } = new UnityEvent();

	/// <summary>
	/// ���� ���� �̺�Ʈ. �ִϸ��̼� ��� �� �̺�Ʈ �߻�
	/// </summary>
	protected override void DiedCharactor()
	{
		base.DiedCharactor();
		onRockCrashed?.Invoke();
	}
}
