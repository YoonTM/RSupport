using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using TMPro;
using Photon.Pun;


/// <summary>
/// 화면의 플레이어를 제어하는 스크립트. Charactor 스크립트를 상속받아 움직임 처리
/// </summary>
public class Player : Charactor
{
	public UnityEvent onPlayerDamaged { get; } = new UnityEvent();
	public UnityEvent onCompleteGame { get; } = new UnityEvent();

	[SerializeField] TextMeshPro infoText = null;

	private bool isBoomDelay = false;
	private float playTime;

	public Vector2 playerMoveDir;

	Rock target = null;

	//위치,속도
	private float speed = 3f;
	private Vector2 speedVelocity;

	//플레이어 스타트 지점
	private Vector2[] randPos = new Vector2[]
	{
    new Vector2(4.5f, 3.5f),
    new Vector2(22.5f, 3.5f),
    new Vector2(40.5f, 3.5f),
    new Vector2(4.5f, 17.5f),
    new Vector2(4.5f, 31.5f),
    new Vector2(22.5f, 31.5f),
    new Vector2(40.5f, 31.5f),
    new Vector2(40.5f, 17.5f),
    new Vector2(37.5f, 11.5f)
	};

	/// <summary>
	/// 캐릭터의 위치 초기화
	/// </summary>
	protected override void InitCharactor()
	{
		base.InitCharactor();

		//시간,위치 초기화
		playTime = 0f;

		if (pv.IsMine)
			pv.RPC("SetRandPos", RpcTarget.All);
		else
			GetComponentInChildren<Camera>().gameObject.SetActive(false);

		speedVelocity = Vector2.zero;
	}

	private void Update()
	{
		playTime += Time.deltaTime;
	}

	/// <summary>
	/// 입력 처리. 서버에서, 본인이 아닌 입력이 들어오면 무시 하도록 처리
	/// </summary>
	void FixedUpdate()
	{
		if (!pv.IsMine)
			return;

		rigidBody.velocity = Vector2.zero;
		speedVelocity = Vector2.zero;
		if (Input.GetKey(KeyCode.W))
		{
			speedVelocity.y = speed;
		}
		if (Input.GetKey(KeyCode.S))
		{
			speedVelocity.y = -speed;
		}
		if (Input.GetKey(KeyCode.D))
		{
			speedVelocity.x = speed;
		}
		if (Input.GetKey(KeyCode.A))
		{
			speedVelocity.x = -speed;
		}
		if (Input.GetKey(KeyCode.Space))
		{
			OnClickBoomBtn();
		}

		if (playerMoveDir != Vector2.zero)
			speedVelocity = playerMoveDir * speed;

		//	이동
		pv.RPC("Move", RpcTarget.All);
	}


	/// <summary>
	/// 입력에 따른 움직임 벡터 설정으로 캐릭터 이동
	/// </summary>
	[PunRPC]
	protected override void Move()
	{
		rigidBody.velocity = speedVelocity;
		base.Move();
	}

	/// <summary>
	/// 폭탄 이벤트 처리. 애니메이션 출력 및 주변 오브젝트 검출
	/// </summary>
	public void OnClickBoomBtn()
	{
		if (!pv.IsMine)
			return;

		animator.SetBool("isClickedBoomBtn", true);
		if (!isBoomDelay)
		{
			pv.RPC("CheckObject", RpcTarget.All);
		}
		Invoke("BoomDelayAnim", 0.1f);
	}

	private void BoomDelayAnim()
	{
		animator.SetBool("isClickedBoomBtn", false);
	}

	/// <summary>
	/// 조이스틱에서 들어오는 움직임 명령 리스너
	/// </summary
	public void changeMoveVec(Vector2 vec)
	{
		playerMoveDir = vec;
	}

	private void RockCrashListener()
	{
		isBoomDelay = false;
	}

	/// <summary>
	/// Collision 입력처리. 적에게 맞으면 피격, 종료 캐릭터에 닿으면 성공, 돌에 닿으면 타겟 처리
	/// </summary>
	private void OnCollisionEnter2D(Collision2D other)
	{
		switch (other.gameObject.tag)
		{
			case "Enemy":
				{
					onPlayerDamaged?.Invoke();
					Invoke("EnemyEvent", 2f);
				}
				break;
			case "Finish":
				{
					GameComplete(playTime);
				}
				break;
			case "Rock":
				{
					target = other.gameObject.GetComponent<Rock>();
				}
				break;
		}
	}

	/// <summary>
	/// 돌과 닿은 부분이 없다면 타겟 해제
	/// </summary>
	private void OnCollisionExit2D(Collision2D other)
	{
		switch (other.gameObject.tag)
		{
			case "Rock":
				{
					target = null;
				}
				break;
		}
	}

	/// <summary>
	/// 폭탄 이벤트 발생 시 타겟이 있는지 체크 후, 타겟에게 죽음 애니메이션 처리
	/// </summary>
	[PunRPC]
	private void CheckObject()
	{
		if (target != null)
		{
			isBoomDelay = true;
			target.DoDieCharactor();
			target.onRockCrashed.AddListener(RockCrashListener);
			target = null;
		}
	}

	/// <summary>
	/// 적에게 피격시 랜덤한 위치로 이동하도록 처리
	/// </summary>
	private void EnemyEvent()
	{
		if (pv.IsMine)
			pv.RPC("SetRandPos", RpcTarget.All);
	}

	/// <summary>
	/// 지정된 랜덤 위치중 한군데로 이동하도록 하는 함수
	/// </summary>
	[PunRPC]
	public void SetRandPos()
	{
		transform.position = randPos[Random.Range(0, randPos.Length)];
	}

	/// <summary>
	/// 게임 성공시 출력될 메세지 처리
	/// </summary>
	public void GameComplete(float playTime)
	{
		StartCoroutine(Complete(playTime));
	}

	IEnumerator Complete(float playTime)
	{
		infoText.text = "도착 했어요!!!\n플레이 시간 : " + string.Format("{0:N2}", playTime) + "초";
		infoText.gameObject.SetActive(true);

		yield return new WaitForSeconds(5f);

		onCompleteGame?.Invoke();
	}
}