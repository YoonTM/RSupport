using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInfo : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Image serverImg = null;
    [SerializeField] Image ownImg = null;
    [SerializeField] TextMeshProUGUI nameTxt = null;

	/// <summary>
	/// 플레이어 정보를 방 정보 화면에 보여줄 함수
	/// </summary>
	/// <param name="isServer">해당 플레이어가 마스터인지 구별</param>
	/// <param name="isOwn">해당 플레이어가 로컬인지 구별</param>
	/// <param name="name">플레이어의 닉네임</param>
    public void SetPlayer(bool isServer, bool isOwn, string name)
    {
        if (isServer)
            if (serverImg)
                serverImg.gameObject.SetActive(true);
        if (isOwn)
            if (ownImg)
                ownImg.gameObject.SetActive(true);
        if (name != string.Empty)
        {
            this.name = name;
            if (nameTxt)
                nameTxt.text = name;
        }
    }
}
