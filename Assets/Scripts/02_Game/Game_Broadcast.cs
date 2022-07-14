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
	/// 자주 사용될 항목을 초기화
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
	/// 종료시 이벤트 정리
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
	/// 게임 시작시 실행될 함수
	/// 플레이어를 서버에 만들고, 플레이어가 발행할 이벤트 구독
	/// 플레이어에게 조이스틱 이벤트 할당
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
	/// 몬스터에게 피격시 렌더링중인 이미지 페이드인,페이드아웃 효과
	/// </summary
	private void PlayerDamagedListener()
	{
		StopAllCoroutines();
		StartCoroutine(ChangeRawImageCor());
	}

	/// <summary>
	/// 시간에 따라 화면 페이드인, 페이드아웃 처리
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
	/// 게임 완료 시 게임 방에서 나감 처리
	/// </summary>
	private void GameCompleteListener()
	{
		LeaveRoomBtn_OnClick();
	}

	/// <summary>
	/// 방 나가기 버튼 리스너
	/// </summary>
	private void LeaveRoomBtn_OnClick()
	{
		PhotonNetwork.LeaveRoom();
	}

	/// <summary>
	/// 방에서 나가졌을때, 0번 씬으로 이동
	/// </summary>
	public override void OnLeftRoom()
	{
		SceneManager.LoadScene(0);
	}

	/// <summary>
	/// 화면의 폭탄 버튼 클릭 시 플레이어에게 이벤트 전달
	/// </summary>
	public void BoomBtn_OnClick()
	{
		if(player)
			player.OnClickBoomBtn();
	}
}