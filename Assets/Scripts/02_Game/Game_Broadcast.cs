using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Photon.Pun;
using UnityEngine.SceneManagement;

public class Game_Broadcast : MonoBehaviourPunCallbacks
{
	[SerializeField] Button leaveRoomBtn = null;
	[SerializeField] GameObject infoPanel = null;
	[SerializeField] Button startGameBtn = null;
	[SerializeField] Game_Joystick joyStick = null;
	[SerializeField] Button boomBtn = null;
	[SerializeField] RawImage rawImage = null;
	[SerializeField] Transform backgroundTrn = null;

	private Player player = null;

	/// <summary>
	/// ���� ���� �׸��� �ʱ�ȭ
	/// </summary>
	protected void Start()
	{
		if (leaveRoomBtn)
			leaveRoomBtn.onClick.AddListener(LeaveRoomBtn_OnClick);
		if (startGameBtn)
			startGameBtn.onClick.AddListener(StartGameBtn_OnClick);
		if (boomBtn)
			boomBtn.onClick.AddListener(BoomBtn_OnClick);
	}

	/// <summary>
	/// ����� �̺�Ʈ ����
	/// </summary>
	private void OnDestroy()
	{
		if (leaveRoomBtn)
			leaveRoomBtn.onClick.RemoveListener(LeaveRoomBtn_OnClick);
		if (startGameBtn)
			startGameBtn.onClick.RemoveListener(StartGameBtn_OnClick);
		if (boomBtn)
			boomBtn.onClick.RemoveListener(BoomBtn_OnClick);
		if (player)
		{
			player.onPlayerDamaged.RemoveListener(PlayerDamagedListener);
			player.onCompleteGame.RemoveListener(GameCompleteListener);
		}
	}

	/// <summary>
	/// ���� ���۽� ����� �Լ�
	/// �÷��̾ ������ �����, �÷��̾ ������ �̺�Ʈ ����
	/// �÷��̾�� ���̽�ƽ �̺�Ʈ �Ҵ�
	/// </summary>
	public void StartGameBtn_OnClick()
	{
		infoPanel.SetActive(false);

		GameObject obj =  PhotonNetwork.Instantiate("Prefabs/Game/Player", Vector3.zero, Quaternion.identity);
		player = obj.GetComponent<Player>();
		if (player)
		{
			if(backgroundTrn)
			{
				player.GetComponent<LoopBackGround>().backGround = Instantiate(backgroundTrn);
			}

			player.onPlayerDamaged.AddListener(PlayerDamagedListener);
			player.onCompleteGame.AddListener(GameCompleteListener);
			player.gameObject.SetActive(true);
			joyStick.moveVecUpdate += player.changeMoveVec;
		}
	}

	/// <summary>
	/// ���Ϳ��� �ǰݽ� ���������� �̹��� ���̵���,���̵�ƿ� ȿ��
	/// </summary
	private void PlayerDamagedListener()
	{
		StopAllCoroutines();
		StartCoroutine(ChangeRawImageCor());
	}

	/// <summary>
	/// �ð��� ���� ȭ�� ���̵���, ���̵�ƿ� ó��
	/// </summary>
	/// <returns></returns>
	IEnumerator ChangeRawImageCor()
	{
		float value = 1f;
		while (value > 0f)
		{
			value -= 5f / 255f;
			rawImage.color = new Color(1, 1, 1, value);
			yield return null;
		}
		yield return new WaitForSeconds(1.5f);
		while (value < 1f)
		{
			value += 5f / 255f;
			rawImage.color = new Color(1, 1, 1, value);
			yield return null;
		}
		yield break;
	}

	/// <summary>
	/// ���� �Ϸ� �� ���� �濡�� ���� ó��
	/// </summary>
	private void GameCompleteListener()
	{
		LeaveRoomBtn_OnClick();
	}

	/// <summary>
	/// �� ������ ��ư ������
	/// </summary>
	private void LeaveRoomBtn_OnClick()
	{
		PhotonNetwork.LeaveRoom();
	}

	/// <summary>
	/// �濡�� ����������, 0�� ������ �̵�
	/// </summary>
	public override void OnLeftRoom()
	{
		SceneManager.LoadScene(0);
	}

	/// <summary>
	/// ȭ���� ��ź ��ư Ŭ�� �� �÷��̾�� �̺�Ʈ ����
	/// </summary>
	public void BoomBtn_OnClick()
	{
		if(player)
			player.OnClickBoomBtn();
	}
}