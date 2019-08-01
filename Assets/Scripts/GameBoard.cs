using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameBoard : MonoBehaviour {
	/*--We are detecting collision system on grid bases 
	 * --and checking each tile if pacMan is on that tile
	 * 
	 * 
	 * 
	*/
	public Text playerText, readyText;

	private static int boardWidth = 28;
	private static int boardHeight= 36;

	private bool didStartDeath = false;

	public int totalPellets = 0;
	public int score = 0;

	public GameObject[,] board = new GameObject[boardWidth, boardHeight]; 

	public AudioClip backgroundNormal, backgroundFrightened, backgroundPacManDeath;

	private bool didStartConsume = false;
	public static bool isPlayerOneUP = true;
	public static int playerOneScore = 0, playerTwoScore = 0;
	public AudioClip consumedGhostAudio;

	public Text highScoreText, playerOneUp, playerTwoUp, playerOneScoreText, playerTwoScoreText;
	public Text consumedGhostScoreText;
	public Image playerLives2, playerLives3;

	public static int playerOneLevel = 1, playerTwoLevel = 1;

	public bool shouldBlink = false;
	public float blinkIntervalTime = 0.1f;
	private float blinkTimer = 0;

	public Sprite mazeBlue, mazeWhite;

	private bool didIncrementLevel = false;

	public static int ghostConsumedRunningScore;

	public Image[] levelImages;

	public bool didSpawnBonusItem1_player1, didSpawnBonusItem2_player1;
	public bool didSpawnBonusItem1_player2, didSpawnBonusItem2_player2;

	// Use this for initialization
	void Start () {
		Object[] objects = GameObject.FindObjectsOfType (typeof(GameObject));
		foreach (GameObject o in objects) {
			Vector2 pos = o.transform.position;

			if (o.name != "PacMan" && o.tag != "Ghost" && o.tag!= "ghostHome" &&
				o.name != "Nodes" && o.name != "NonNodes" && o.name != "Maze" &&
				o.name != "Pellets" &&	o.name != "Canvas" && o.tag != "UIElements") {

				if (o.GetComponent<Tile> () != null) {
					if (o.GetComponent<Tile> ().isPellet || o.GetComponent<Tile> ().isSuperPellet) {
						totalPellets++;
					}
				}

				board [(int)pos.x, (int)pos.y] = o;
			} else {
				Debug.Log ("Found PacMan at: " + pos);
			}
		}
		if (isPlayerOneUP) {
			if (playerOneLevel == 1) {					//--Play intro music only for level one
				GetComponent<AudioSource> ().Play ();		
			}
		} else {
			if (playerTwoLevel == 1) {
				GetComponent<AudioSource> ().Play ();
			}
		}
		StartGame ();
	}

	void Update() {
		UpdateUI ();
		CheckPelletsConsumed ();
		CheckShouldBlink ();
		BonusItems ();
	}

	void BonusItems() {
		if (GameMenu.isOnePlayerGame) {		//one player game
			SpawnBonusItemForPlayer (1);
		} else {
			if (isPlayerOneUP) {
				SpawnBonusItemForPlayer (1);
			} else {
				SpawnBonusItemForPlayer (2);
			}
		}
	}

	void SpawnBonusItemForPlayer(int playerNum) {
		if (playerNum == 1) {
			if(GameMenu.playerOnePelletsConsumed >= 70 && GameMenu.playerOnePelletsConsumed < 170) {
				if (!didSpawnBonusItem1_player1) {
					didSpawnBonusItem1_player1 = true;
					SpawnBonusItemForLevel (playerOneLevel);
				}
			} else if(GameMenu.playerOnePelletsConsumed >= 170) {
				if (!didSpawnBonusItem2_player1) {
					didSpawnBonusItem2_player1 = true;
					SpawnBonusItemForLevel (playerOneLevel);
				}
			}
		} else {
			if(GameMenu.playerTwoPelletsConsumed >= 70 && GameMenu.playerTwoPelletsConsumed < 170) {
				if (!didSpawnBonusItem1_player2) {
					didSpawnBonusItem1_player2 = true;
					SpawnBonusItemForLevel (playerTwoLevel);
				}
			} else if(GameMenu.playerTwoPelletsConsumed >= 170) {
				if (!didSpawnBonusItem2_player2) {
					didSpawnBonusItem2_player2 = true;
					SpawnBonusItemForLevel (playerTwoLevel);
				}
			}
		}
	}

	void SpawnBonusItemForLevel(int level)  {
		GameObject bonusItem = null;
		if (level == 1)
			bonusItem = Resources.Load ("Prefabs/bonus_cherries", typeof(GameObject)) as GameObject;
		else if (level == 2)
			bonusItem = Resources.Load ("Prefabs/bonus_strawberry", typeof(GameObject)) as GameObject;
		else if (level == 3)
			bonusItem = Resources.Load ("Prefabs/bonus_peach", typeof(GameObject)) as GameObject;
		else if (level == 4)
			bonusItem = Resources.Load ("Prefabs/bonus_peach", typeof(GameObject)) as GameObject;
		else if (level == 5)
			bonusItem = Resources.Load ("Prefabs/bonus_apple", typeof(GameObject)) as GameObject;
		else if (level == 6)
			bonusItem = Resources.Load ("Prefabs/bonus_apple", typeof(GameObject)) as GameObject;
		else if (level == 7)
			bonusItem = Resources.Load ("Prefabs/bonus_grapes", typeof(GameObject)) as GameObject;
		else if (level == 8)
			bonusItem = Resources.Load ("Prefabs/bonus_grapes", typeof(GameObject)) as GameObject;
		else if (level == 9)
			bonusItem = Resources.Load ("Prefabs/bonus_galaxian", typeof(GameObject)) as GameObject;
		else if (level == 10)
			bonusItem = Resources.Load ("Prefabs/bonus_galaxian", typeof(GameObject)) as GameObject;
		else if (level == 11)
			bonusItem = Resources.Load ("Prefabs/bonus_bell", typeof(GameObject)) as GameObject;
		else if (level == 12)
			bonusItem = Resources.Load ("Prefabs/bonus_bell", typeof(GameObject)) as GameObject;
		else
			bonusItem = Resources.Load ("Prefabs/bonus_key", typeof(GameObject)) as GameObject;

		Instantiate (bonusItem);
	}

	void UpdateUI(){
		playerOneScoreText.text = playerOneScore.ToString ();
		playerTwoScoreText.text = playerTwoScore.ToString ();

		int currentLevel;

		if (isPlayerOneUP) {
			currentLevel = playerOneLevel;
			if (GameMenu.livesPlayerOne == 3) {					//managing renderer of lifes of Pacman
				playerLives3.enabled = true;
				playerLives2.enabled = true;
			} else if (GameMenu.livesPlayerOne == 2) {
				playerLives3.enabled = true;
				playerLives2.enabled = false;
			} else if (GameMenu.livesPlayerOne == 1) {
				playerLives3.enabled = false;
				playerLives2.enabled = false;
			}
		} else {
			currentLevel = playerTwoLevel;
			if (GameMenu.livesPlayerTwo == 3) {					//managing renderer of lifes of Pacman
				playerLives3.enabled = true;
				playerLives2.enabled = true;
			} else if (GameMenu.livesPlayerTwo == 2) {
				playerLives3.enabled = true;
				playerLives2.enabled = false;
			} else if (GameMenu.livesPlayerTwo == 1) {
				playerLives3.enabled = false;
				playerLives2.enabled = false;
			}
		}

		for (int i = 0; i < levelImages.Length; i++) {
			Image li = levelImages [i];
			li.enabled = false;
		}
		for (int i = 1; i < levelImages.Length + 1; i++) {
			if (currentLevel >= i) {		//--managing currentLevel with Ii array 
				Image li = levelImages [i-1];
				li.enabled = true;
			}
		}
	}

	void CheckPelletsConsumed() {

		if (isPlayerOneUP) {		//player one is playing
			if(totalPellets == GameMenu.playerOnePelletsConsumed){
				PlayerWin (1);
			}
		} else {					//player two is playing
			if(totalPellets == GameMenu.playerTwoPelletsConsumed){
				PlayerWin (2);
			}
		}
	}

	void PlayerWin(int playerNum) {
		if (playerNum == 1) {
			if(!didIncrementLevel) {
				didIncrementLevel = true;	//--used to keep a tab if level incremented or not and breaks below concept
				playerOneLevel++;			//--This updates repeatedly as per the update status
				StartCoroutine (ProcessWin (2f));
			}
		} else {
			if(!didIncrementLevel) {
				didIncrementLevel = true;
				playerTwoLevel++;			//--This updates repeatedly as per the update status
				StartCoroutine (ProcessWin (2f));
			}
		}
	}

	IEnumerator ProcessWin(float delay) {
		GameObject pacMan = GameObject.Find ("PacMan");
		pacMan.transform.GetComponent<PacMan> ().canMove = false;
		pacMan.transform.GetComponent<Animator> ().enabled = false;
		transform.GetComponent<AudioSource>().Stop ();

		GameObject[] o = GameObject.FindGameObjectsWithTag ("Ghost");
		foreach (GameObject ghost in o) {
			ghost.transform.GetComponent<Ghost> ().canMove = false;
			ghost.transform.GetComponent<Animator> ().enabled = false;
		}
		yield return new WaitForSeconds (delay);
		StartCoroutine (BlinkBoard (2f));
	}

	IEnumerator BlinkBoard(float delay) {
		GameObject pacMan = GameObject.Find ("PacMan");
		pacMan.transform.GetComponent<SpriteRenderer> ().enabled = false;
		GameObject[] o = GameObject.FindGameObjectsWithTag ("Ghost");
		foreach (GameObject ghost in o) {
			ghost.transform.GetComponent<SpriteRenderer> ().enabled= false;
		}
		//--Blink the Board
		shouldBlink = true;
		yield return new WaitForSeconds (delay);

		//--Restart the game at next level
		shouldBlink = false;
		StartNextLevel ();
	}

	//--Managing the Next-Levels
	private void StartNextLevel() {
		StopAllCoroutines();
		//--Manage all the pellets of both the players when shifting to next-levels
		if (isPlayerOneUP) {
			ResetPelletesForPlayer (1);
			GameMenu.playerOnePelletsConsumed = 0;
			didSpawnBonusItem1_player1 = false;
			didSpawnBonusItem2_player1 = false;
		} else {
			ResetPelletesForPlayer (2);
			GameMenu.playerTwoPelletsConsumed = 0;
			didSpawnBonusItem1_player2 = false;
			didSpawnBonusItem2_player2 = false;
		}
		//--to make sure that after stopping the blinking the maze color stays to blue only
		GameObject.Find ("Maze").transform.GetComponent<SpriteRenderer> ().sprite = mazeBlue;
		didIncrementLevel = false;
	 	StartCoroutine (ProcessStartNextLevel (1));
 	}

	IEnumerator ProcessStartNextLevel(float delay) {
		playerText.transform.GetComponent<Text> ().enabled = true;
		readyText.transform.GetComponent<Text> ().enabled = true;

		if (isPlayerOneUP)
			StartCoroutine (StartBlinking (playerOneUp));
		else
			StartCoroutine (StartBlinking (playerTwoUp));
		RedrawBoard ();
		yield return new WaitForSeconds (delay);
		StartCoroutine (ProcessRestartShowObjects (1));
	}

	private void CheckShouldBlink() {
		if (shouldBlink) {
			if (blinkTimer < blinkIntervalTime) {
				blinkTimer += Time.deltaTime;
			} else {
				blinkTimer = 0;
				if (GameObject.Find ("Maze").transform.GetComponent<SpriteRenderer> ().sprite == mazeBlue) {
					GameObject.Find ("Maze").transform.GetComponent<SpriteRenderer> ().sprite = mazeWhite;
		 		} else {
					GameObject.Find ("Maze").transform.GetComponent<SpriteRenderer> ().sprite = mazeBlue;
				}
			}
		}
	}

	public void StartGame() {
		//--Hide all the ghosts and PacMan
		if (GameMenu.isOnePlayerGame) {								//--One player game
			playerTwoUp.GetComponent<Text> ().enabled = false;
			playerTwoScoreText.GetComponent<Text> ().enabled = false;
		} else {
			playerTwoUp.GetComponent<Text> ().enabled = true;
			playerTwoScoreText.GetComponent<Text> ().enabled = true;
		}

		if (isPlayerOneUP) {						//--Player one playing
			StartCoroutine (StartBlinking (playerOneUp));
		} else {
			StartCoroutine (StartBlinking (playerTwoUp));
		}
		//-Hide all ghosts and pacman until music ends
		GameObject[] o = GameObject.FindGameObjectsWithTag ("Ghost");
		foreach (GameObject ghost in o) {
			ghost.transform.GetComponent<SpriteRenderer> ().enabled = false;
			ghost.transform.GetComponent<Ghost> ().canMove = false;
		}

		GameObject pacMan = GameObject.Find ("PacMan");
		pacMan.transform.GetComponent<SpriteRenderer> ().enabled = false;
		pacMan.transform.GetComponent<PacMan> ().canMove = false;

		StartCoroutine (ShowObjectsAfter (2.25f));
	}

	public void StartConsumed(Ghost consumedGhost) {
		if (!didStartConsume) {
			didStartConsume = true;

			//--Pause all he ghosts
			GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");
			foreach (GameObject ghost in o) {
				ghost.transform.GetComponent<Ghost> ().canMove = false;
			}

			//--Pause PacMan
			GameObject pacMan = GameObject.Find("PacMan");
			pacMan.transform.GetComponent<PacMan> ().canMove = false;

			//--Hide Pacman
			pacMan.transform.GetComponent<SpriteRenderer> ().enabled = false;

			//--Hide the consumed ghost
			consumedGhost.transform.GetComponent<SpriteRenderer>().enabled = false;

			//-Stop background music
			transform.GetComponent<AudioSource> ().Stop ();

			Vector2 pos = consumedGhost.transform.position;
			Vector2 viewPortPoint = Camera.main.WorldToViewportPoint (pos);

			consumedGhostScoreText.GetComponent<RectTransform> ().anchorMin = viewPortPoint;
			consumedGhostScoreText.GetComponent<RectTransform> ().anchorMax = viewPortPoint;

			consumedGhostScoreText.text = ghostConsumedRunningScore.ToString ();
			consumedGhostScoreText.GetComponent<Text> ().enabled = true;

			//--play the audio
			transform.GetComponent<AudioSource> ().PlayOneShot (consumedGhostAudio);

			//--wait for audio to finish
			StartCoroutine(ProcessConsumedAfter(0.75f, consumedGhost));
		}
	}

	public void StartConsumedBonusItem (GameObject bonusItem, int scoreValue) {
		Vector2 pos = bonusItem.transform.position;
		Vector2 viewPortPoint = Camera.main.WorldToViewportPoint (pos);

		consumedGhostScoreText.GetComponent<RectTransform>().anchorMin = viewPortPoint;
		consumedGhostScoreText.GetComponent<RectTransform>().anchorMax = viewPortPoint;

		consumedGhostScoreText.text = scoreValue.ToString ();
		consumedGhostScoreText.GetComponent<Text> ().enabled = true;
		Destroy (bonusItem.gameObject);
		StartCoroutine (ProcessConsumedBonusItem (0.75f));
	}

	IEnumerator ProcessConsumedBonusItem (float delay) {
		yield return new WaitForSeconds (delay);
		consumedGhostScoreText.GetComponent<Text> ().enabled = false;
	}

	IEnumerator StartBlinking (Text blinkText) {
		yield return new WaitForSeconds (0.25f);
		blinkText.GetComponent<Text> ().enabled = !blinkText.GetComponent<Text> ().enabled;
		StartCoroutine (StartBlinking (blinkText));
	}

	IEnumerator ProcessConsumedAfter(float delay, Ghost consumedGhost) {
		yield return new WaitForSeconds (delay);
		//--hide the score
		consumedGhostScoreText.GetComponent<Text> ().enabled = false;

		//--show pacman
		GameObject pacMan = GameObject.Find ("PacMan");
		pacMan.transform.GetComponent<SpriteRenderer> ().enabled = true;

		//--show consumedghost
		consumedGhost.transform.GetComponent<SpriteRenderer> ().enabled = true;

		//--resume ghosts
		GameObject[] o = GameObject.FindGameObjectsWithTag ("Ghost");
		foreach (GameObject ghost in o) {
			ghost.transform.GetComponent<Ghost> ().canMove = true;
		}
		//--resume pacman
		pacMan.transform.GetComponent<PacMan> ().canMove= true;
		//--start sound
		transform.GetComponent<AudioSource> ().Play ();

		didStartConsume = false;

	}

	IEnumerator ShowObjectsAfter(float delay) {
		yield return new WaitForSeconds (delay);
		GameObject[] o = GameObject.FindGameObjectsWithTag ("Ghost");
		foreach (GameObject ghost in o) {
			ghost.transform.GetComponent<SpriteRenderer> ().enabled = true;
		}

		GameObject pacMan = GameObject.Find ("PacMan");
		pacMan.transform.GetComponent<SpriteRenderer> ().enabled = true;

		playerText.transform.GetComponent<Text> ().enabled = false;

		StartCoroutine (StartGameAfter(2f));
	}

	IEnumerator StartGameAfter(float delay) {
		yield return new WaitForSeconds (delay);
		GameObject[] o = GameObject.FindGameObjectsWithTag ("Ghost");
		foreach (GameObject ghost in o) {
			ghost.transform.GetComponent<Ghost> ().canMove = true;
		}

		GameObject pacMan = GameObject.Find ("PacMan");
		pacMan.transform.GetComponent<PacMan> ().canMove = true;
		readyText.transform.GetComponent<Text> ().enabled = false;

		transform.GetComponent<AudioSource>	().clip = backgroundNormal;
		transform.GetComponent<AudioSource>	().Play ();
	}

	public void StartDeath() {
		if (!didStartDeath) {

			StopAllCoroutines ();

			if (GameMenu.isOnePlayerGame) {
				playerOneUp.GetComponent<Text> ().enabled = true;
			} else {
				playerOneUp.GetComponent<Text> ().enabled = true;
				playerTwoUp.GetComponent<Text> ().enabled = true;
			}

			GameObject bonusItem = GameObject.Find ("bonusItem");
			if (bonusItem) {
				Destroy (bonusItem.gameObject);
			}

			didStartDeath = true;
			GameObject[] o = GameObject.FindGameObjectsWithTag ("Ghost");
			foreach (GameObject ghost in o) {
				ghost.transform.GetComponent<Ghost> ().canMove = false;
			}

			GameObject pacMan = GameObject.Find ("PacMan");
			pacMan.transform.GetComponent<PacMan> ().canMove = false;
			pacMan.transform.GetComponent<Animator> ().enabled = false;
			transform.GetComponent<AudioSource> ().Stop();

			StartCoroutine (ProcessDeathAfter (2f));
		}
	}

	//--Coroutine to actual routines
	IEnumerator ProcessDeathAfter(float delay) {
		yield return new WaitForSeconds (delay);
		GameObject[] o = GameObject.FindGameObjectsWithTag ("Ghost");
		foreach (GameObject ghost in o) {
			ghost.transform.GetComponent<SpriteRenderer> ().enabled = false;
		}

		StartCoroutine (ProcessDeathAnimation (1.9f));
	}

	IEnumerator ProcessDeathAnimation (float delay) {
		GameObject pacMan = GameObject.Find ("PacMan");
		pacMan.transform.localScale = new Vector3 (1, 1, 1);
		pacMan.transform.localRotation = Quaternion.Euler(0, 0, 0);

		pacMan.transform.GetComponent<Animator> ().runtimeAnimatorController = pacMan.transform.GetComponent<PacMan> ().deathAnimation;
		pacMan.transform.GetComponent<Animator> ().enabled = true;

		transform.GetComponent<AudioSource> ().clip = backgroundPacManDeath;
		transform.GetComponent<AudioSource> ().Play ();

		yield return new WaitForSeconds (delay);

		StartCoroutine (ProcessRestart(1f));
	}

	IEnumerator ProcessRestart (float delay) {
		///pacManLives -= 1;
	
		if (isPlayerOneUP)
			GameMenu.livesPlayerOne -= 1;
		else
			GameMenu.livesPlayerTwo -= 1;
				
		if (GameMenu.livesPlayerOne == 0 && GameMenu.livesPlayerTwo == 0) {
			playerText.transform.GetComponent<Text> ().enabled = true;
			readyText.transform.GetComponent<Text> ().text = "GAME OVER";
			readyText.transform.GetComponent<Text> ().color = new Color (255f / 255f, 165f / 255f, 0f / 255f);

			readyText.transform.GetComponent<Text> ().enabled = true;
			GameObject pacMan = GameObject.Find ("PacMan");
			pacMan.transform.GetComponent<SpriteRenderer> ().enabled = false;
			transform.GetComponent<AudioSource> ().Stop ();
			StartCoroutine (ProcessGameOver (2f));

		} else if(GameMenu.livesPlayerOne == 0 || GameMenu.livesPlayerTwo == 0) {
			//--One of the player has ended all the lives of the and only other player plays

			if (GameMenu.livesPlayerOne == 0) {
				playerText.transform.GetComponent<Text> ().text = "PLAYER 1";
			} else if (GameMenu.livesPlayerTwo == 0) {
				playerText.transform.GetComponent<Text> ().text = "PLAYER 2";
			}
			readyText.transform.GetComponent<Text>().text = "GAME OVER";
			readyText.transform.GetComponent<Text>().enabled = true;
			playerText.transform.GetComponent<Text> ().enabled = true;

			GameObject pacMan = GameObject.Find ("PacMan");
			pacMan.transform.GetComponent<SpriteRenderer> ().enabled = false;
			transform.GetComponent<AudioSource> ().Stop ();
			//StartCoroutine (ProcessGameOver (2));
			yield return new WaitForSeconds(delay);

			if (!GameMenu.isOnePlayerGame)
				isPlayerOneUP = !isPlayerOneUP;

			if (isPlayerOneUP)
				StartCoroutine (StartBlinking(playerOneUp));
			else 
				StartCoroutine (StartBlinking(playerTwoUp));

			RedrawBoard ();

			if(isPlayerOneUP)
				playerText.transform.GetComponent<Text>().text = "PLAYER 1";
			else
				playerText.transform.GetComponent<Text>().text = "PLAYER 2";

			readyText.transform.GetComponent<Text> ().text = "READY!";

			yield return new WaitForSeconds (delay);

			StartCoroutine (ProcessRestartShowObjects (2));

		} else {				//--Game is not over and one played died and its the turn of the other player
			playerText.transform.GetComponent<Text> ().enabled = true;
			readyText.transform.GetComponent<Text> ().enabled = true;
			GameObject pacMan = GameObject.Find ("PacMan");
			pacMan.transform.GetComponent<SpriteRenderer> ().enabled = false;
			transform.GetComponent<AudioSource> ().Stop ();

			if(!GameMenu.isOnePlayerGame)
				isPlayerOneUP = !isPlayerOneUP;

			if (isPlayerOneUP)
				StartCoroutine (StartBlinking(playerOneUp));
			else 
				StartCoroutine (StartBlinking(playerTwoUp));

			if (!GameMenu.isOnePlayerGame) {
				if (isPlayerOneUP)
					playerText.transform.GetComponent<Text> ().text = "PLAYER 1";
				else
					playerText.transform.GetComponent<Text> ().text = "PLAYER 2";
			}

			RedrawBoard ();
			yield return new WaitForSeconds (delay);
			StartCoroutine (ProcessRestartShowObjects (1f));
		}
	}

	IEnumerator ProcessGameOver(float delay) {
		yield return new WaitForSeconds (delay);
		SceneManager.LoadScene ("GameMenu");
	}

	IEnumerator ProcessRestartShowObjects (float delay) {
		playerText.transform.GetComponent<Text> ().enabled = false;
		GameObject[] o = GameObject.FindGameObjectsWithTag ("Ghost");
		foreach (GameObject ghost in o) {
			ghost.transform.GetComponent<SpriteRenderer> ().enabled = true;
			ghost.transform.GetComponent<Animator> ().enabled = true;
			ghost.transform.GetComponent<Ghost> ().MoveToStartPosition ();
		}

		GameObject pacMan = GameObject.Find ("PacMan");
		pacMan.transform.GetComponent<Animator> ().enabled = false;
		pacMan.transform.GetComponent<SpriteRenderer> ().enabled = true;
		pacMan.transform.GetComponent<PacMan> ().MoveToStartPosition ();

		yield return new WaitForSeconds (delay);
		Restart ();
	}


	public void Restart() {

		int playerLevel = 0;

		if (isPlayerOneUP)
			playerLevel = playerOneLevel;
		else if(!isPlayerOneUP)
			playerLevel = playerTwoLevel;

		GameObject.Find ("PacMan").GetComponent<PacMan> ().SetDifficultyForLevel (playerLevel);
		GameObject[] obj = GameObject.FindGameObjectsWithTag ("Ghost");
		foreach (GameObject ghost in obj) {
			ghost.transform.GetComponent<Ghost> ().SetDifficultyForLevel (playerLevel);
		}

		readyText.transform.GetComponent<Text> ().enabled = false;

		GameObject pacMan = GameObject.Find ("PacMan");
		pacMan.transform.GetComponent<PacMan> ().Restart ();

		GameObject[] o = GameObject.FindGameObjectsWithTag ("Ghost");
		foreach (GameObject ghost in o) {
			ghost.transform.GetComponent<Ghost> ().Restart ();
		}
		transform.GetComponent<AudioSource> ().clip = backgroundNormal;
		transform.GetComponent<AudioSource> ().Play ();
		didStartDeath = false;
	}

	/*--Below are the 2 functions used for the management of board and
	**--the pellets simultaneously when both the players are playing 
	**--
	*/
	void ResetPelletesForPlayer(int playerNum) {
		Object[] objects = GameObject.FindObjectsOfType (typeof(GameObject));
		foreach (GameObject o in objects) {
			if (o.GetComponent<Tile> () != null) {
				if (o.GetComponent<Tile> ().isPellet || o.GetComponent<Tile> ().isSuperPellet) {
					if (playerNum == 1) {
						//--diable all the pellets that playerOne has already consumed before dying
						o.GetComponent<Tile> ().didConsumePlayerOne = false;
					} else {
						//--diable all the pellets that playerTwo has already consumed before dying
						o.GetComponent<Tile> ().didConsumePlayerTwo = false;
					}
				}
			}
		}
	}
	//--Draw the Board as the player(number) has left before dying for another player
	void RedrawBoard() {
		Object[] objects = GameObject.FindObjectsOfType (typeof(GameObject));
		foreach (GameObject o in objects) {
			if (o.GetComponent<Tile> () != null) {
				if (o.GetComponent<Tile> ().isPellet || o.GetComponent<Tile> ().isSuperPellet) {
					if (isPlayerOneUP) {
						if (o.GetComponent<Tile> ().didConsumePlayerOne)
							o.GetComponent<SpriteRenderer> ().enabled = false;
						else
							o.GetComponent<SpriteRenderer> ().enabled = true;
					} else {
						if (o.GetComponent<Tile> ().didConsumePlayerTwo)
							o.GetComponent<SpriteRenderer> ().enabled = false;
						else
							o.GetComponent<SpriteRenderer> ().enabled = true;
					}
				}
			}
		}
	}
}
