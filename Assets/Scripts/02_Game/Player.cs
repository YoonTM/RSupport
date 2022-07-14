using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using TMPro;
using Photon.Pun;


/// <summary>
/// ȭ���� �÷��̾ �����ϴ� ��ũ��Ʈ. Charactor ��ũ��Ʈ�� ��ӹ޾� ������ ó��
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

	//��ġ,�ӵ�
	private float speed = 3f;
	private Vector2 speedVelocity;

	//�÷��̾� ��ŸƮ ����
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
	/// ĳ������ ��ġ �ʱ�ȭ
	/// </summary>
	protected override void InitCharactor()
	{
		base.InitCharactor();

		//�ð�,��ġ �ʱ�ȭ
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
	/// �Է� ó��. ��������, ������ �ƴ� �Է��� ������ ���� �ϵ��� ó��
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

		//	�̵�
		pv.RPC("Move", RpcTarget.All);
	}


	/// <summary>
	/// �Է¿� ���� ������ ���� �������� ĳ���� �̵�
	/// </summary>
	[PunRPC]
	protected override void Move()
	{
		rigidBody.velocity = speedVelocity;
		base.Move();
	}

	/// <summary>
	/// ��ź �̺�Ʈ ó��. �ִϸ��̼� ��� �� �ֺ� ������Ʈ ����
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
	/// ���̽�ƽ���� ������ ������ ��� ������
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
	/// Collision �Է�ó��. ������ ������ �ǰ�, ���� ĳ���Ϳ� ������ ����, ���� ������ Ÿ�� ó��
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
	/// ���� ���� �κ��� ���ٸ� Ÿ�� ����
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
	/// ��ź �̺�Ʈ �߻� �� Ÿ���� �ִ��� üũ ��, Ÿ�ٿ��� ���� �ִϸ��̼� ó��
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
	/// ������ �ǰݽ� ������ ��ġ�� �̵��ϵ��� ó��
	/// </summary>
	private void EnemyEvent()
	{
		if (pv.IsMine)
			pv.RPC("SetRandPos", RpcTarget.All);
	}

	/// <summary>
	/// ������ ���� ��ġ�� �ѱ����� �̵��ϵ��� �ϴ� �Լ�
	/// </summary>
	[PunRPC]
	public void SetRandPos()
	{
		transform.position = randPos[Random.Range(0, randPos.Length)];
	}

	/// <summary>
	/// ���� ������ ��µ� �޼��� ó��
	/// </summary>
	public void GameComplete(float playTime)
	{
		StartCoroutine(Complete(playTime));
	}

	IEnumerator Complete(float playTime)
	{
		infoText.text = "���� �߾��!!!\n�÷��� �ð� : " + string.Format("{0:N2}", playTime) + "��";
		infoText.gameObject.SetActive(true);

		yield return new WaitForSeconds(5f);

		onCompleteGame?.Invoke();
	}
}