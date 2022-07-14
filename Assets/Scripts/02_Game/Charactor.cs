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
	/// 자주 사용될 항목을 초기화
	/// </summary>
	protected virtual void InitCharactor()
	{
		//물리, 애니메이션 초기화
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
	/// 캐릭터의 죽음 애니매이션을 호출
	/// </summary>
	public virtual void DoDieCharactor()
	{
		animator.SetBool("doDie", true);
	}

	/// <summary>
	/// 캐릭터 죽음 애니메이션 종료 이벤트
	/// </summary>
	protected virtual void DiedCharactor()
	{
		gameObject.SetActive(false);
	}
	
	/// <summary>
	/// 캐릭터의 움직임을 제어하는 함수
	/// </summary>
	[PunRPC]
	protected virtual void Move()
	{
		//	움직임이 존재 한다면, 걷는 애니메이션 출력
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