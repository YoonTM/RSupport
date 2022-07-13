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

	public virtual void DoDieCharactor()
	{
		animator.SetBool("doDie", true);
	}

	protected virtual void DiedCharactor()
	{
		gameObject.SetActive(false);
	}
	
	[PunRPC]
	protected virtual void Move()
	{
		//걷는 애니메이션 
		if (rigidBody.velocity.x == 0 && rigidBody.velocity.y == 0)
		{
			animator.SetBool("isWalking", false);
		}
		else
		{
			animator.SetBool("isWalking", true);
		}

		if (rigidBody.velocity.x < 0)
		{
			spriteRenderer.flipX = true;
		}
		else if (rigidBody.velocity.x > 0)
		{
			spriteRenderer.flipX = false;
		}
	}
}