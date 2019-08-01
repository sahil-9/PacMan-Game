﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour {

	public static bool isOnePlayerGame = true;

	public Text playerText1, playerText2, playerSelector;

	public static int livesPlayerOne, livesPlayerTwo;
	public static int playerOnePelletsConsumed = 0, playerTwoPelletsConsumed = 0;
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp (KeyCode.UpArrow)) {
			if (!isOnePlayerGame) {
				isOnePlayerGame = true;
				playerSelector.transform.localPosition = new Vector3 (playerSelector.transform.localPosition.x,
					playerText1.transform.localPosition.y, playerSelector.transform.localPosition.z);
			}
		
		} else if (Input.GetKeyUp (KeyCode.DownArrow)) {
			if (isOnePlayerGame) {
				isOnePlayerGame = false;
				playerSelector.transform.localPosition = new Vector3 (playerSelector.transform.localPosition.x,
					playerText2.transform.localPosition.y, playerSelector.transform.localPosition.z);
			}
		} else if (Input.GetKeyUp (KeyCode.Return)) {

			livesPlayerOne = 3;
			livesPlayerTwo = 3;
			if (isOnePlayerGame)
				livesPlayerTwo = 0;
			SceneManager.LoadScene ("Level1");
		}
	}
}