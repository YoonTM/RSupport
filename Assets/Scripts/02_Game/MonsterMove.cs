using System.Collections;
using UnityEngine;
using Photon.Pun;
public class MonsterMove : Charactor
{
	private Coroutine thinkCo = null;

	public int nextMoveX;
	public int nextMoveY;

	protected override void InitCharactor()
	{
		base.InitCharactor();
		thinkCo = StartCoroutine(Think());
	}

	private void FixedUpdate()
	{
		if (!pv.IsMine)
			return;
		pv.RPC("Move", RpcTarget.All);
	}

	[PunRPC]
	protected override void Move()
	{
		rigidBody.velocity = new Vector2(nextMoveX, nextMoveY);
		base.Move();
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		switch (other.gameObject.tag)
		{
			case "Platform":
				{
					StopCoroutine(thinkCo);
					thinkCo = StartCoroutine(Think());
				}
				break;
			case "Player":
				{
					PlayerHit();
				}
				break;
		}
	}

	private IEnumerator Think(float waitTime = 1)
	{
		yield return new WaitForSeconds(waitTime);

		nextMoveX = Random.Range(-1, 2);
		nextMoveY = Random.Range(-1, 2);

		thinkCo = StartCoroutine(Think(3));

		yield break;
	}

	private void StopAttack()
	{
		animator.SetBool("isAttack", false);
	}

	private void PlayerHit()
	{
		StopCoroutine(thinkCo);
		CancelInvoke();
		nextMoveX = 0;
		nextMoveY = 0;
		animator.SetBool("isAttack", true);
		Invoke("StopAttack", 2f);
		thinkCo = StartCoroutine(Think(2));
	}
}