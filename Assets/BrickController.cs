using UnityEngine;
using System.Collections;

public class BrickController : MonoBehaviour {

	private Vector3 mousePosition;
	//public float moveSpeed = 0.1f;
	private float height = 3.26f;
	public bool isPlayerOneTurn;
	public bool isActive;
	private GameObject curBrick;
	public GameObject p1Brick;
	public GameObject p2Brick;
	[Range(0f,5f)]
	public float distanceX = 1.36f;
	private float slot1x = -4.1f;
	public float[] gridWidth;

	private enum Field {
		empty,
		playerone,
		playertwo
	}
	private Field[,] grid;

	// Use this for initialization
	void Start () {
		transform.position = new Vector3 (0, height, transform.position.z);
		gridWidth = new float[] {slot1x,slot1x+distanceX ,slot1x+distanceX*2,slot1x+distanceX*3,slot1x+distanceX*4,slot1x+distanceX*5,
			slot1x+distanceX*6 };
		grid = new Field[7, 6];
	}

	// Update is called once per frame
	void Update () {
		if (isActive) {
			mousePosition = Input.mousePosition;
			mousePosition = Camera.main.ScreenToWorldPoint (mousePosition);
			//Vector2 newPosition = Vector2.Lerp (transform.position, new Vector2 (mousePosition.x, height), moveSpeed);
			Vector3 newPosition = new Vector3 (mousePosition.x, height, 1);
			if (newPosition.x <= -4.15f) {
				newPosition.x = -4.15f;
			} 
			if (newPosition.x >= 4.15f) {
				newPosition.x = 4.15f;
			}
			curBrick.transform.position = newPosition;
			int column = CheckBoundaryAndSetPosX ();

			if (Input.GetMouseButtonDown (0)) {
				isActive = false;
				DropBrick ();
				bool isGameOver = CheckValidPos (column);
				if (!isGameOver) 
					StartCoroutine(GetComponent<GameManager> ().endPlayerTurn ());
			}
		}

	}

	public bool CheckValidPos(int column) {
		for (int i = grid.GetLength(1) - 1; i >= 0; i--) {
			if (grid [column, i] == Field.empty) {
				grid [column, i] = isPlayerOneTurn ? Field.playerone : Field.playertwo;
				Debug.Log ("Placing player brick at depth: " + (i + 1));
				if (LightAlgorithm (column, i)) {
					declareWinner (isPlayerOneTurn);
					return true;
				}
				break;
			}
		}
		return false;
	}

	void declareWinner(bool p1Won) {
		GetComponent<GameManager> ().setVictory (p1Won);
	}

	//I got this algorithm off the internet
	bool LightAlgorithm(int indexX, int indexY) {
		for(int x = -1; x < 2; x++){
			for (int y = -1; y < 2; y++) {
				if (grid.GetLength (0) > indexX + x && indexX + x >= 0 && grid.GetLength (1) > indexY + y && indexY + y >= 0) {
					if (x != 0 || y != 0) {
						if (grid [indexX, indexY] == grid [indexX + x, indexY + y]) {
							int currentPoints = 0;
							for (int j = -3; j < 4; j++) {
								if (grid.GetLength (0) > indexX + (x * j) && indexX + (x * j) >= 0 && grid.GetLength (1) > indexY + (y * j) && indexY + (y * j) >= 0) {
									if (grid [indexX, indexY] == grid [indexX + (x * j), indexY + (y * j)]) {
										currentPoints++;

										if (currentPoints > 3) {
											return true;
										}
									} else {
										currentPoints = 0;
									}
								}
							}
						}
					}
				}
			}
		}
		return false;
	}

	int CheckBoundaryAndSetPosX() {
		int idx = 0;
		float minDistance = 100f;
		for (int i = 0; i < gridWidth.Length; i++) {
			if (curBrick.transform.position.x > gridWidth [i]) {
				if (minDistance > curBrick.transform.position.x - gridWidth [i]) {
					idx = i;
					minDistance = curBrick.transform.position.x - gridWidth [idx];
				} 
			} else {
				if (minDistance > gridWidth [i] - curBrick.transform.position.x) {
					idx = i;
					minDistance = gridWidth [idx] - curBrick.transform.position.x;
				} 
			}
		}

		curBrick.transform.position = new Vector3 (gridWidth[idx], curBrick.transform.position.y, curBrick.transform.position.z);
		return idx;
	}

	void DropBrick() {
		curBrick.GetComponent<Rigidbody> ().useGravity = true;
	}

	public void StartPlayerTurn(bool isP1) {
		isActive = true;
		isPlayerOneTurn = isP1;
		curBrick = Instantiate ( isPlayerOneTurn ? p1Brick : p2Brick, new Vector3 (0, height, transform.position.z), 
			(isPlayerOneTurn ? p1Brick.transform.rotation : p2Brick.transform.rotation));
	}
}