using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using TMPro;
using Photon.Pun;
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

    [PunRPC]
    protected override void Move()
    {
        rigidBody.velocity = speedVelocity;
        base.Move();
    }

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

    public void changeMoveVec(Vector2 vec)
    {
        playerMoveDir = vec;
    }

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

    private void RockCrashListener()
    {
        isBoomDelay = false;
    }

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

    private void EnemyEvent()
    {
        if (pv.IsMine)
            pv.RPC("SetRandPos", RpcTarget.All);
    }

    [PunRPC]
    public void SetRandPos()
    {
        transform.position = randPos[Random.Range(0, randPos.Length)];
    }

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