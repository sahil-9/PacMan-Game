using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public bool isPortal;
	public bool isPellet;
	public bool isSuperPellet;
	public bool didConsumePlayerOne, didConsumePlayerTwo;		//used to store if the pellet is consumed or not

	public GameObject portalReceiver;

	public bool isGhostHouseEntrance;
	public bool isGhostHouse;

	public bool isBonusItem;	//
	public int pointValue;		//points as per the bonus item
}
