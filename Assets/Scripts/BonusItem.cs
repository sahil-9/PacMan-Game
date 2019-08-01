using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusItem : MonoBehaviour {
	float randomLifeExpectancy;
	float currentLifeTime;

	/*--Have to add the bous items on runtime and into the grid array,
	 *--to check for the collision with pacman
	 *--2 bonus items per round
	 * 1) 70 pellets consumed 
	 * 2) 170 pellets consumed
	 * 
	 * --2 ways to implement
	 * --1) using rect or using arrays
	*/
	// Use this for initialization
	void Start () {
		randomLifeExpectancy = Random.Range (9, 10);		//display time of bonusItem
		this.name = "bonusItem";
		GameObject.Find ("Game").GetComponent<GameBoard> ().board [14, 13] = this.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if (currentLifeTime < randomLifeExpectancy)
			currentLifeTime += Time.deltaTime;
		else
			Destroy (this.gameObject);
	}
}