﻿using UnityEngine;
using System.Collections;

public class SpawnController : MonoBehaviour {

	public int totalAICharacter ;
	public float timeToSpawnChar ;

	public GameObject[] AIChar;

	private float tTS;
	private int numOfChild;
	private int count;

	// Use this for initialization
	void Start () {
		totalAICharacter = totalAICharacter * 2;
		numOfChild = this.transform.childCount;
		tTS = Time.time;
		count = numOfChild;
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Time.time - tTS >= timeToSpawnChar) {
			numOfChild = this.transform.childCount;
			//Debug.Log(numOfChild);
			if(numOfChild < totalAICharacter){
				int x = Random.Range(0, AIChar.Length);
				var go = Instantiate(AIChar[x],this.transform.position ,Quaternion.identity)as GameObject;
				go.name = "AIChar_" + count + "_" + this.name;
				go.transform.parent = transform;
				count++;
			}
			tTS = Time.time;
		}

	}
}
