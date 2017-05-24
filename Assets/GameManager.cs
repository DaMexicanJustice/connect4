using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	private GameState curGameState;
	public bool isPlayerOne;
	private BrickController bc;
	public Text winText;
	public float waitfor;
	public Text buttonText;

	// Use this for initialization
	void Start () {
		curGameState = GameState.startGame;	
		bc = GetComponent<BrickController> ();
	}
	
	// Update is called once per frame
	void Update () {
		switch (curGameState) {
		case GameState.startGame:
			if (Random.value > 0.5f) {
				isPlayerOne = false;
			} else {
				isPlayerOne = true;
			}
			curGameState = GameState.playerturn;
			break;
		case GameState.playerturn:
			if (!bc.isActive) {
				bc.StartPlayerTurn (isPlayerOne);
			}
			break;
		case GameState.calculationStep:
			// Idle. Actual calculation is taking place in BrickController. See LightAlgorithm()
			break;
		case GameState.victory:
			winText.text = "Player " + (isPlayerOne ? "1" : "2") + " won!";
			buttonText.text = "Quit Game";
			Time.timeScale = 0.5f;
			break;
		}
	}

	public IEnumerator endPlayerTurn() {
		curGameState = GameState.calculationStep;
		yield return new WaitForSeconds (waitfor);
		curGameState = GameState.playerturn;
		isPlayerOne = !isPlayerOne;
	}

	public void setVictory(bool p1Won) {
		isPlayerOne = p1Won;
		curGameState = GameState.victory;
	}
}
