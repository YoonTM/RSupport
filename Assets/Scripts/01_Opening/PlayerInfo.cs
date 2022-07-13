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
