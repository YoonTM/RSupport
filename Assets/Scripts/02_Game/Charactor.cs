using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Charactor : MonoBehaviour
{
	protected PhotonView pv = null;
	protected Rigidbody2D rigidBody;
	protected SpriteRenderer spriteRenderer;
	protected Animator animator;

	private void Awake()
	{
		InitCharactor();
	}

	/// <summary>
	/// ���� ���� �׸��� �ʱ�ȭ
	/// </summary>
	protected virtual void InitCharactor()
	{
		//����, �ִϸ��̼� �ʱ�ȭ
		if (!rigidBody)
		{
			if( GetComponent<Rigidbody2D>() != null )
				rigidBody = GetComponent<Rigidbody2D>();
		}
		if (!spriteRenderer)
			spriteRenderer = GetComponent<SpriteRenderer>();
		if (!animator)
			animator = GetComponent<Animator>();
		if(!pv)
			pv = GetComponent<PhotonView>();
	}

	/// <summary>
	/// ĳ������ ���� �ִϸ��̼��� ȣ��
	/// </summary>
	public virtual void DoDieCharactor()
	{
		animator.SetBool("doDie", true);
	}

	/// <summary>
	/// ĳ���� ���� �ִϸ��̼� ���� �̺�Ʈ
	/// </summary>
	protected virtual void DiedCharactor()
	{
		gameObject.SetActive(false);
	}
	
	/// <summary>
	/// ĳ������ �������� �����ϴ� �Լ�
	/// </summary>
	[PunRPC]
	protected virtual void Move()
	{
		//	�������� ���� �Ѵٸ�, �ȴ� �ִϸ��̼� ���
		if (rigidBody.velocity.x == 0 && rigidBody.velocity.y == 0)
		{
			animator.SetBool("isWalking", false);
		}
		else
		{
			animator.SetBool("isWalking", true);
		}
	}
}