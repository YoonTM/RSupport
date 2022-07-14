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
    /// �ν����Ϳ� ���� �̺�Ʈ ����
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
    /// �ν����Ϳ� ���� �̺�Ʈ ����
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
    /// ���� �������� ���¸� �ؽ�Ʈ�� ǥ���ϴ� �Լ�.
    /// </summary>
    private void FixedUpdate()
    {
        serverStateTxt.text = PhotonNetwork.NetworkClientState.ToString();
    }

    /// <summary>
    ///  ���� ���� ��ư Ŭ���� ����Ǵ� �Լ�.
    ///  �г��� ��ǲ�ʵ带 Ȯ���Ͽ� ����ڸ� ������ �� �ֵ��� �ȳ�
    ///  �г��� ���� �� �ڵ� �� ��ũ ��� ����.
    /// </summary>
    private void ServerConnectBtn_OnClick()
    {
        if (nickNameInpF.text == string.Empty)
        {
            nickNameInpF.placeholder.GetComponent<TextMeshProUGUI>().text = "�г����� �Է� �� ������ �������ּ���.";
            return;
        }
        PhotonNetwork.NickName = nickNameInpF.text;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    /// <summary>
    /// ���� ���� ��ư Ŭ���� ����Ǵ� �Լ�.
    /// </summary>
    private void ServerDisconnectBtn_OnClick()
    {
        PhotonNetwork.Disconnect();
    }

    /// <summary>
    ///  ���� ���� �Ϸ�� UI����.
    ///  �г���, �������� ��ư false
    ///  ���̸�, �������� ��ư true
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
    /// ���� ���� ���� �Ϸ�� UI����.
    ///  ���̸�, �������� ��ư false
    ///  �г���, �������� ��ư true
    /// </summary>
    /// <param name="cause"> ���� ���� State ���� </param>  
    public override void OnDisconnected(DisconnectCause cause)
    {
        serverConnectBtn.gameObject.SetActive(true);
        nickNameInpF.gameObject.SetActive(true);

        roomNameInpF.gameObject.SetActive(false);
        serverDisconnectBtn.gameObject.SetActive(false);
        joinOrCreateRoomBtn.gameObject.SetActive(false);
    }

    /// <summary>
    /// �� �̸� ��ǲ�ʵ带 Ȯ���Ͽ� �ش� �濡 �����ϰų�, ���� ���ٸ� �� ����
    /// </summary>
    private void JoinOrCreateRoomBtn_OnClick()
    {
        if (roomNameInpF.text == string.Empty)
        {
            roomNameInpF.placeholder.GetComponent<TextMeshProUGUI>().text = "���̸��� �Է� �� �濡 �������ּ���.";
            return;
        }
        PhotonNetwork.JoinOrCreateRoom(roomNameInpF.text, null, null);
    }

    /// <summary>
    ///  �� ���� �Ϸ� �� UI ����
    ///  �� �̸�, ������ UI false
    ///  �� ���� �� �� ���� ���� ���� UI True
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
    /// �� ������ �÷��̾� ����� ���ΰ�ħ
    /// </summary>
    /// <param name="newPlayer">���� �߰��� �÷��̾�</param>
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        RenewPlayer();
    }

    /// <summary>
    /// �� ������ �÷��̾� ����� ���ΰ�ħ
    /// </summary>
    /// <param name="otherPlayer">������ �÷��̾�</param>
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        RenewPlayer();
    }

	/// <summary>
	/// �÷��̾� ���ΰ�ħ �Լ�.
	/// ���� �÷��̾� ����� ���� �� ��, �� ����� �޾� Prefab ����
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
	/// ���� ���� �Լ�
	/// </summary>
    private void StartGameBtn_OnClick()
    {
        if( !PhotonNetwork.IsMasterClient)
            return;

        PhotonNetwork.LoadLevel("02_Game");
    }
}
