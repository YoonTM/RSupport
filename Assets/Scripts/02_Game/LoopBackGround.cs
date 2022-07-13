using System;
using UnityEngine;

public class LoopBackGround : MonoBehaviour
{
	[SerializeField] private float landSizeX = 9f;
	[SerializeField] private float landSizeY = 7f;
	[SerializeField] private float halfSight = 4.5f;
	private float playerX=0;
	private float playerY=0;
    private Vector2[] border = null;

	[SerializeField] private Transform[] _landArray = null;
	public Transform[] landArray
	{
		get { return _landArray; }
		set { _landArray = value; }
	}
	[SerializeField] private Transform _backGround = null;
	public Transform backGround
	{
		get { return _backGround; }
		set {
            playerX = transform.position.x;
            playerY = transform.position.y;
            _backGround = value;
			landArray = new Transform[_backGround.childCount];
			for (int i = 0; i < _backGround.childCount; i++)
			{
				landArray[i] = _backGround.GetChild(i);
                landArray[i].position = new Vector2(landArray[i].position.x + playerX - 4.5f, landArray[i].position.y + playerY - 3.5f);
            }
		}
	}


	private void Start()
	{
		border = new Vector2[]
		{
	    new Vector2(playerX-landSizeX * 1.5f, playerY+landSizeY * 1.5f ),
	    new Vector2(playerX+landSizeX * 1.5f, playerY-landSizeY * 1.5f )
		};
	}
	
	private void Update()
	{
		BoundaryCheck();
	}

	private void BoundaryCheck()
	{
        if (border == null)
            return;

        if (backGround == null)
            return;

		//오른쪽 체크
		if (border[1].x < transform.position.x + halfSight)
		{
			border[0] += Vector2.right * landSizeX;
			border[1] += Vector2.right * landSizeX;
			MoveTile(0);
		}
		//왼쪽 체크
		else if (border[0].x > transform.position.x - halfSight)
		{
			border[0] -= Vector2.right * landSizeX;
			border[1] -= Vector2.right * landSizeX;
			MoveTile(2);
		}
		//위쪽 체크
		else if (border[0].y < transform.position.y + halfSight)
		{
			border[0] += Vector2.up * landSizeY;
			border[1] += Vector2.up * landSizeY;
			MoveTile(1);
		}
		//아래쪽 체크
		else if (border[1].y > transform.position.y - halfSight)
		{
			border[0] -= Vector2.up * landSizeY;
			border[1] -= Vector2.up * landSizeY;
			MoveTile(3);
		}
	}
	private void MoveTile(int dir)
	{
		Transform[] tempArray = new Transform[9];
		Array.Copy(landArray, tempArray, 9);

		switch (dir)
		{
			case 0:
				//왼쪽타일 오른쪽 이동
				for (int i = 0; i < 9; i++)
				{
					int revise = i - 3;

					if (revise < 0)
					{
						landArray[9 + revise] = tempArray[i];
						tempArray[i].position += Vector3.right * landSizeX * 3;
					}
					else
					{
						landArray[revise] = tempArray[i];
					}
				}
				break;
			case 1:
				//아래타일 위로 이동
				for (int i = 0; i < 9; i++)
				{
					int revise = i % 3;

					if (revise == 2)
					{
						landArray[i - 2] = tempArray[i];
						tempArray[i].position += Vector3.up * landSizeY * 3;
					}
					else
					{
						landArray[i + 1] = tempArray[i];
					}
				}
				break;
			case 2:
				//오른쪽타일 왼쪽으로 이동
				for (int i = 0; i < 9; i++)
				{
					int revise = i + 3;
					if (revise > 8)
					{
						landArray[revise - 9] = tempArray[i];
						tempArray[i].position -= Vector3.right * landSizeX * 3;
					}
					else
					{
						landArray[revise] = tempArray[i];
					}
				}
				break;
			case 3:
				//위타일 아래로 이동
				for (int i = 0; i < 9; i++)
				{
					int revise = i % 3;
					if (revise == 0)
					{
						landArray[i + 2] = tempArray[i];
						tempArray[i].position -= Vector3.up * landSizeY * 3;
					}
					else
					{
						landArray[i - 1] = tempArray[i];
					}
				}
				break;
		}
	}
}