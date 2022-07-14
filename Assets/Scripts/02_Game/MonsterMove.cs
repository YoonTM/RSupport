using System.Collections;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// 화면의 몬스터를 제어하는 스크립트. Charactor 스크립트를 상속받아 움직임 처리
/// </summary>
public class MonsterMove : Charactor
{
	private Coroutine thinkCo = null;

	public int nextMoveX;
	public int nextMoveY;

	/// <summary>
	/// 시작시 움직임 방향을 결정하는 Coroutine 실행
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
	/// Collision에 부딛친 상대 태그를 구별하여 행동
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
	/// 몬스터의 움직임 방향을 설정하는 함수
	/// 랜덤하게 움직이도록 설정
	/// </summary>
	/// <param name="waitTime">움직임 전 대기시간</param>
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
	/// 플레이어와 맞닿는 이벤트 처리
	/// 움직임 정지 후 타격 애니메이션 출력
	/// 타격 이후 움직임 Coroutine 재실행
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