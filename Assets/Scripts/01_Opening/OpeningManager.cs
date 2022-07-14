using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Photon.Pun;
using Photon.Realtime;


public class OpeningManager : MonoBehaviourPunCallbacks
{ 
    [Header("UI")]
    [SerializeField] TMP_InputField nickNameInpF = null;
    [SerializeField] TMP_InputField roomNameInpF = null;

    [SerializeField] Button serverConnectBtn = null;
    [SerializeField] Button serverDisconnectBtn = null;
   
    [SerializeField] Button joinOrCreateRoomBtn = null;
    [SerializeField] Button startGameBtn = null;

    [SerializeField] GameObject roomInfoPanel = null;
    [SerializeField] TextMeshProUGUI roomNameTxt = null;
    [SerializeField] GameObject playerInfoPanel = null;
    [SerializeField] TextMeshProUGUI serverStateTxt = null;

    private readonly string gameVersion = "1";


    /// <summary>
    /// 인스펙터에 따른 이벤트 세팅
    /// </summary>
    private void Start()
    {
        if (serverConnectBtn)
            serverConnectBtn.onClick.AddListener(ServerConnectBtn_OnClick);
        if (serverDisconnectBtn)
            serverDisconnectBtn.onClick.AddListener(ServerDisconnectBtn_OnClick);

        if (joinOrCreateRoomBtn)
            joinOrCreateRoomBtn.onClick.AddListener(JoinOrCreateRoomBtn_OnClick);
        if (startGameBtn)
            startGameBtn.onClick.AddListener(StartGameBtn_OnClick);

        PhotonNetwork.GameVersion = gameVersion;
    }

    /// <summary>
    /// 인스펙터에 따른 이벤트 세팅
    /// </summary>
    private void OnDestroy()
    {
        if (serverConnectBtn)
            serverConnectBtn.onClick.RemoveListener(ServerConnectBtn_OnClick);
        if (serverDisconnectBtn)
            serverDisconnectBtn.onClick.RemoveListener(ServerDisconnectBtn_OnClick);
        if (joinOrCreateRoomBtn)
            joinOrCreateRoomBtn.onClick.RemoveListener(JoinOrCreateRoomBtn_OnClick);
        if (startGameBtn)
            startGameBtn.onClick.RemoveListener(StartGameBtn_OnClick);
    }

    /// <summary>
    /// 현재 서버접속 상태를 텍스트로 표시하는 함수.
    /// </summary>
    private void FixedUpdate()
    {
        serverStateTxt.text = PhotonNetwork.NetworkClientState.ToString();
    }

    /// <summary>
    ///  서버 접속 버튼 클릭시 시행되는 함수.
    ///  닉네임 인풋필드를 확인하여 사용자를 구별할 수 있도록 안내
    ///  닉네임 설정 및 자동 씬 싱크 모드 설정.
    /// </summary>
    private void ServerConnectBtn_OnClick()
    {
        if (nickNameInpF.text == string.Empty)
        {
            nickNameInpF.placeholder.GetComponent<TextMeshProUGUI>().text = "닉네임을 입력 후 서버에 입장해주세요.";
            return;
        }
        PhotonNetwork.NickName = nickNameInpF.text;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    /// <summary>
    /// 접속 해제 버튼 클릭시 시행되는 함수.
    /// </summary>
    private void ServerDisconnectBtn_OnClick()
    {
        PhotonNetwork.Disconnect();
    }

    /// <summary>
    ///  서버 접속 완료시 UI변경.
    ///  닉네임, 서버접속 버튼 false
    ///  방이름, 게임접속 버튼 true
    /// </summary>
    public override void OnConnectedToMaster()
    {
        nickNameInpF.gameObject.SetActive(false);
        serverConnectBtn.gameObject.SetActive(false);

        roomNameInpF.gameObject.SetActive(true);
        serverDisconnectBtn.gameObject.SetActive(true);
        joinOrCreateRoomBtn.gameObject.SetActive(true);
    }

    /// <summary>
    /// 서버 접속 해제 완료시 UI변경.
    ///  방이름, 게임접속 버튼 false
    ///  닉네임, 서버접속 버튼 true
    /// </summary>
    /// <param name="cause"> 접속 해제 State 종류 </param>  
    public override void OnDisconnected(DisconnectCause cause)
    {
        serverConnectBtn.gameObject.SetActive(true);
        nickNameInpF.gameObject.SetActive(true);

        roomNameInpF.gameObject.SetActive(false);
        serverDisconnectBtn.gameObject.SetActive(false);
        joinOrCreateRoomBtn.gameObject.SetActive(false);
    }

    /// <summary>
    /// 방 이름 인풋필드를 확인하여 해당 방에 접속하거나, 방이 없다면 방 생성
    /// </summary>
    private void JoinOrCreateRoomBtn_OnClick()
    {
        if (roomNameInpF.text == string.Empty)
        {
            roomNameInpF.placeholder.GetComponent<TextMeshProUGUI>().text = "방이름을 입력 후 방에 입장해주세요.";
            return;
        }
        PhotonNetwork.JoinOrCreateRoom(roomNameInpF.text, null, null);
    }

    /// <summary>
    ///  방 접속 완료 시 UI 변경
    ///  방 이름, 방접속 UI false
    ///  방 정보 및 방 내의 유저 정보 UI True
    /// </summary>
    public override void OnJoinedRoom()
    {
        roomNameInpF.gameObject.SetActive(false);
        serverDisconnectBtn.gameObject.SetActive(false);
        joinOrCreateRoomBtn.gameObject.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
            startGameBtn.gameObject.SetActive(true);
        
        roomInfoPanel.SetActive(true);
        roomNameTxt.text = "Room : " + PhotonNetwork.CurrentRoom.Name;

        RenewPlayer();
    }

    /// <summary>
    /// 방 내부의 플레이어 변경시 새로고침
    /// </summary>
    /// <param name="newPlayer">새로 추가된 플레이어</param>
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        RenewPlayer();
    }

    /// <summary>
    /// 방 내부의 플레이어 변경시 새로고침
    /// </summary>
    /// <param name="otherPlayer">퇴장한 플레이어</param>
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        RenewPlayer();
    }

	/// <summary>
	/// 플레이어 새로고침 함수.
	/// 기존 플레이어 목록을 삭제 한 후, 새 목록을 받아 Prefab 복사
	/// </summary>
    private void RenewPlayer()
    {
        for (int i = playerInfoPanel.transform.childCount - 1; i >= 0; i--)
            Destroy(playerInfoPanel.transform.GetChild(i).gameObject);

        foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
        {
            PlayerInfo newPlayerObj = Instantiate(Resources.Load<PlayerInfo>("Prefabs/UI/PlayerInfo"), playerInfoPanel.transform);
            newPlayerObj.SetPlayer(p.IsMasterClient, p.IsLocal, p.NickName);
        }
    }

	/// <summary>
	/// 게임 실행 함수
	/// </summary>
    private void StartGameBtn_OnClick()
    {
        if( !PhotonNetwork.IsMasterClient)
            return;

        PhotonNetwork.LoadLevel("02_Game");
    }
}
