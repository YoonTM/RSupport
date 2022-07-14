using System.Collections;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// ȭ���� ���͸� �����ϴ� ��ũ��Ʈ. Charactor ��ũ��Ʈ�� ��ӹ޾� ������ ó��
/// </summary>
public class MonsterMove : Charactor
{
	private Coroutine thinkCo = null;

	public int nextMoveX;
	public int nextMoveY;

	/// <summary>
	/// ���۽� ������ ������ �����ϴ� Coroutine ����
	/// </summary>
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

	/// <summary>
	/// Collision�� �ε�ģ ��� �±׸� �����Ͽ� �ൿ
	/// </summary
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

	/// <summary>
	/// ������ ������ ������ �����ϴ� �Լ�
	/// �����ϰ� �����̵��� ����
	/// </summary>
	/// <param name="waitTime">������ �� ���ð�</param>
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

	/// <summary>
	/// �÷��̾�� �´�� �̺�Ʈ ó��
	/// ������ ���� �� Ÿ�� �ִϸ��̼� ���
	/// Ÿ�� ���� ������ Coroutine �����
	/// </summary>
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